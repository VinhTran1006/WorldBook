// ==================== CHATBOT CONFIGURATION ====================
let userConn;
let isInAdminChat = false;

// DOM Elements
const toggleBtn = document.getElementById("chatbot-toggle");
const chatBox = document.getElementById("chatbot-box");
const closeBtn = document.getElementById("chatbot-close");
const sendBtn = document.getElementById("chatbot-send");
const input = document.getElementById("chatbot-input");
const messages = document.getElementById("chatbot-messages");
const suggestBox = document.getElementById("chatbot-suggestions");
const contactBtn = document.getElementById("contact-admin");

const defaultSuggestions = [
    "Hôm nay có sách nào đang sale?",
    "Sách nào vừa mới được ra mắt?",
    "Cách đặt sách ở trang web?",
    "Giới thiệu về Website WorldBook Shop"
];

let suggestions = [...defaultSuggestions];

// ==================== PROFANITY FILTER ====================
const bannedWords = [
    "cặc", "lồn", "đụ", "đỉ", "súc vật",
    "lol", "fuck", "đm", "dm", "cc", "clm", "vl", "cl", "địt", "quốc"
];

function filterProfanity(text) {
    let filtered = text;

    bannedWords.forEach(word => {
        const regex = new RegExp(`\\b${word}\\b`, "gi");
        const replacement = "*".repeat(word.length);
        filtered = filtered.replace(regex, replacement);
    });

    return filtered;
}

// ==================== SPAM PROTECTION ====================
let userMessageCount = 0;
let adminMessageCount = 0;
let isSpamBlocked = false;
let spamBlockEndTime = 0;

function checkSpam() {
    const now = Date.now();

    if (isSpamBlocked && now < spamBlockEndTime) {
        const remainingTime = Math.ceil((spamBlockEndTime - now) / 1000);
        alert(`Bạn đã nhắn quá nhanh! Vui lòng chờ ${remainingTime} giây trước khi nhắn tiếp.`);
        return false;
    }

    if (isSpamBlocked && now >= spamBlockEndTime) {
        isSpamBlocked = false;
        userMessageCount = 0;
        console.log("Spam block hết hạn");
        return true;
    }

    return true;
}

function handleSpam() {
    if (userMessageCount >= 9 && adminMessageCount === 0) {
        console.log(`SPAM DETECTED! User: ${userMessageCount}, Admin: ${adminMessageCount}`);
        isSpamBlocked = true;
        spamBlockEndTime = Date.now() + (49 * 1000);

        const div = document.createElement("div");
        div.className = "message bot";
        div.innerHTML = `
            <i class="bi bi-exclamation-triangle-fill text-danger"></i>
            <div class="bubble bg-danger text-white">
                <strong>Cảnh báo:</strong> Bạn đã nhắn quá nhanh (spam)! Tài khoản bị chặn trong 49 giây.
            </div>`;
        messages.appendChild(div);
        messages.scrollTop = messages.scrollHeight;
        saveChatState();

        return false;
    }

    return true;
}

function onAdminMessageReceived() {
    adminMessageCount++;
    userMessageCount = 0;
    console.log(`Admin nhắn. Reset user counter. Admin: ${adminMessageCount}, User: 0`);
}

// ==================== CHAT FUNCTIONS ====================
function showWelcomeMessage() {
    messages.innerHTML = `
        <div class="message bot">
            <i class="bi bi-robot"></i>
            <div class="bubble">
                Chào mừng bạn đến với <b>WorldBook Shop</b>!<br/>
                Bạn cần tui tư vấn gì hông nè
            </div>
        </div>`;
    renderSuggestions();
    saveChatState();
}

function renderSuggestions() {
    suggestBox.innerHTML = "";

    if (suggestions.length === 0 || isInAdminChat) {
        suggestBox.style.display = "none";
        return;
    }

    suggestBox.style.display = "flex";
    suggestions.forEach(q => {
        const btn = document.createElement("button");
        btn.className = "chat-suggestion";
        btn.textContent = q;
        btn.onclick = () => handleSuggestionClick(q);
        suggestBox.appendChild(btn);
    });

    saveChatState();
}

function addMessage(role, text) {
    const div = document.createElement("div");
    div.className = `message ${role}`;

    // 🆕 Lọc từ ngữ thô tục cho user, không lọc admin
    const displayText = role === "user" ? filterProfanity(text) : text;

    div.innerHTML = role === "bot"
        ? `<i class="bi bi-robot"></i><div class="bubble">${displayText}</div>`
        : `<div class="bubble">${displayText}</div><i class="bi bi-person-circle"></i>`;
    messages.appendChild(div);
    messages.scrollTop = messages.scrollHeight;
    saveChatState();
}

async function handleSuggestionClick(text) {
    suggestions = suggestions.filter(s => s !== text);
    renderSuggestions();
    await sendMessage(text);
}

async function sendMessage(forcedText = null) {
    const msg = forcedText || input.value.trim();
    if (!msg) return;

    if (!checkSpam()) {
        return;
    }

    if (isInAdminChat) {
        userMessageCount++;
        console.log(`User nhắn lần ${userMessageCount}`);

        if (!handleSpam()) {
            input.value = "";
            return;
        }
    }

    addMessage("user", msg);
    input.value = "";

    const typingEl = document.createElement("div");
    typingEl.className = "typing";
    typingEl.innerHTML = "<span>.</span><span>.</span><span>.</span>";
    messages.appendChild(typingEl);
    messages.scrollTop = messages.scrollHeight;

    try {
        if (isInAdminChat) {
            const userNameEl = document.querySelector("strong");
            const userName = userNameEl ? userNameEl.textContent.trim() : "Khách";

            // 🆕 Lọc từ thô tục trước khi gửi admin
            const filteredMsg = filterProfanity(msg);

            console.log(`Sending to admin as "${userName}": ${filteredMsg}`);
            await userConn.invoke("SendMessageToAdmin", userName, filteredMsg);
            typingEl.remove();
        } else {
            const res = await fetch("/chat/ask", {
                method: "POST",
                headers: { "Content-Type": "application/x-www-form-urlencoded" },
                body: "message=" + encodeURIComponent(msg)
            });

            const data = await res.json();
            typingEl.remove();
            addMessage("bot", data.reply || "Xin lỗi, AI hiện không thể trả lời được");

            if (suggestions.length > 0) renderSuggestions();
            else suggestBox.style.display = "none";
        }
    } catch (err) {
        console.error("Lỗi:", err);
        typingEl.remove();
        addMessage("bot", "Xin lỗi, có lỗi xảy ra");
    }
}

function saveChatState() {
    const state = {
        messages: messages.innerHTML,
        suggestions: suggestions,
        isInAdminChat: isInAdminChat
    };
    sessionStorage.setItem("worldbook_chat_state_v1", JSON.stringify(state));
}

function loadChatState() {
    const state = sessionStorage.getItem("worldbook_chat_state_v1");
    if (state) {
        try {
            const parsed = JSON.parse(state);
            messages.innerHTML = parsed.messages || "";
            suggestions = parsed.suggestions || [...defaultSuggestions];
            isInAdminChat = parsed.isInAdminChat || false;
            renderSuggestions();
        } catch (err) {
            console.error("Error loading chat state:", err);
        }
    }
}

// ==================== EVENT LISTENERS ====================
toggleBtn.onclick = () => {
    chatBox.style.display = "flex";
    toggleBtn.style.display = "none";
    loadChatState();
    if (messages.innerHTML.trim() === "") showWelcomeMessage();
};

closeBtn.onclick = () => {
    chatBox.style.display = "none";
    toggleBtn.style.display = "flex";
    saveChatState();
};

sendBtn.onclick = () => sendMessage();
input.addEventListener("keypress", e => {
    if (e.key === "Enter") sendMessage();
});

// ==================== SIGNALR CONNECTION ====================
document.addEventListener("DOMContentLoaded", () => {
    userConn = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .withAutomaticReconnect()
        .build();

    userConn.start()
        .then(() => console.log("SignalR User connected!"))
        .catch(err => console.error("❌ SignalR error:", err));

    // 🆕 Nhận tin nhắn từ Admin - SETUP TRƯỚC KHI CLICK
    userConn.on("ReceiveAdminMessage", function (message) {
        console.log(`Received from admin: ${message}`);
        isInAdminChat = true;
        onAdminMessageReceived();

        const div = document.createElement("div");
        div.className = "message admin";
        div.innerHTML = `
            <i class="bi bi-person-badge-fill text-danger"></i>
            <div class="bubble bg-warning-subtle border border-warning">
                <strong>Admin:</strong> ${message}
            </div>`;
        const chatMessages = document.getElementById("chatbot-messages");
        if (chatMessages) {
            chatMessages.appendChild(div);
            chatMessages.scrollTop = chatMessages.scrollHeight;
        }
        saveChatState();
    });

    // 🆕 Khi user bấm "Liên hệ admin"
    if (contactBtn) {
        contactBtn.addEventListener("click", async () => {
            if (contactBtn.disabled) return;

            contactBtn.disabled = true;
            contactBtn.innerHTML = "<i class='bi bi-hourglass-split'></i> Đang gửi yêu cầu...";

            const userNameEl = document.querySelector("strong");
            const userName = userNameEl ? userNameEl.textContent.trim() : "Khách";
            const message = "Tui cần admin tư vấn!";

            addMessage("user", message);

            try {
                isInAdminChat = true;
                await userConn.invoke("StartAdminChat", userName);
                await userConn.invoke("NotifyAdmin", userName, message);

                addMessage("bot", "Đã gửi yêu cầu đến admin, bạn đợi chút nghen");
                contactBtn.classList.add("btn-secondary");
                contactBtn.innerHTML = "<i class='bi bi-check-circle'></i> Đã liên hệ admin";

                suggestBox.style.display = "none";
                saveChatState();
            } catch (err) {
                console.error("Gửi thất bại:", err);
                addMessage("bot", "Có lỗi khi gửi yêu cầu, bạn thử lại nghen");
                contactBtn.disabled = false;
                contactBtn.classList.remove("btn-secondary");
                contactBtn.innerHTML = "<i class='bi bi-person-lines-fill'></i> Liên hệ Admin";
                isInAdminChat = false;
            }
        });
    }
});

// Kiểm tra đường dẫn hiện tại
const currentPath = window.location.pathname;
const chatContainer = document.getElementById("chatbot-container");
if (currentPath === "/" || currentPath.startsWith("/Book/GetBookDetails")) {
    chatContainer.style.display = "block";
} else {
    chatContainer.style.display = "none";
}
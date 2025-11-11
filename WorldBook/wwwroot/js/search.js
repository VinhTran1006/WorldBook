// Search Functionality with Auto-suggestions
$(document).ready(function () {
    const searchInput = $('#searchInput');
    const suggestionsBox = $('#searchSuggestions');
    let debounceTimer;

    // Load cart count on page load
    updateCartCount();

    // Auto-suggestions on input
    searchInput.on('input', function () {
        clearTimeout(debounceTimer);
        const term = $(this).val().trim();

        if (term.length < 2) {
            suggestionsBox.removeClass('show').empty();
            return;
        }

        debounceTimer = setTimeout(function () {
            fetchSuggestions(term);
        }, 300);
    });

    // Fetch suggestions from server
    function fetchSuggestions(term) {
        $.ajax({
            url: '/Search/Suggestions',
            type: 'GET',
            data: { term: term },
            success: function (data) {
                displaySuggestions(data);
            },
            error: function () {
                console.error('Error fetching suggestions');
            }
        });
    }

    // Display suggestions
    function displaySuggestions(suggestions) {
        suggestionsBox.empty();

        if (suggestions.length === 0) {
            suggestionsBox.removeClass('show');
            return;
        }

        suggestions.forEach(function (item) {
            const suggestionItem = $('<div>')
                .addClass('suggestion-item')
                .html('<i class="bi bi-search"></i>' + escapeHtml(item))
                .on('click', function () {
                    searchInput.val(item);
                    suggestionsBox.removeClass('show').empty();
                    $('.search-form').submit();
                });

            suggestionsBox.append(suggestionItem);
        });

        suggestionsBox.addClass('show');
    }

    // Close suggestions when clicking outside
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.search-container').length) {
            suggestionsBox.removeClass('show').empty();
        }
    });

    // Close suggestions on ESC key
    searchInput.on('keydown', function (e) {
        if (e.key === 'Escape') {
            suggestionsBox.removeClass('show').empty();
        }
    });

    // Update cart count
    function updateCartCount() {
        $.ajax({
            url: '/Cart/GetCartCount',
            type: 'GET',
            success: function (data) {
                const count = data.count || 0;
                $('#cartBadge').text(count);

                if (count > 0) {
                    $('#cartBadge').show();
                } else {
                    $('#cartBadge').hide();
                }
            },
            error: function () {
                console.error('Error fetching cart count');
            }
        });
    }

    // Expose updateCartCount globally
    window.updateCartCount = updateCartCount;

    // Helper function to escape HTML
    function escapeHtml(text) {
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return text.replace(/[&<>"']/g, function (m) { return map[m]; });
    }

    // Smooth scroll for anchor links
    $('a[href^="#"]').on('click', function (e) {
        const target = $(this.getAttribute('href'));
        if (target.length) {
            e.preventDefault();
            $('html, body').animate({
                scrollTop: target.offset().top - 80
            }, 800);
        }
    });

    // Add loading indicator for forms
    $('.search-form').on('submit', function () {
        $('.btn-search').html('<span class="spinner-border spinner-border-sm"></span>');
    });
});
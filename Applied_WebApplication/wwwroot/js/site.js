// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function initDropdown(selector, id, dropdownList) {
    $(selector).on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $(dropdownList).show();
        $(dropdownList).find(".dropdown-item").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });

    $(dropdownList).on("click", ".dropdown-item", function () {
        var name = $(this).text();
        var selectedId = $(this).data("id");

        $(selector).val(name);
        $(id).val(selectedId);

        $(dropdownList).hide();
    });

    $(document).on("click", function (event) {
        if (!$(event.target).closest(selector).length) {
            $(dropdownList).hide();
        }
    });
}

function ddownClick(caretIconSelector, dropdownListSelector, searchInputSelector) {
    $(caretIconSelector).on("click", function (e) {
        e.stopPropagation();
        $(dropdownListSelector).toggle();
    });

    $(document).on("click", function (event) {
        if (!$(event.target).closest(`${dropdownListSelector}, ${caretIconSelector}, ${searchInputSelector}`).length) {
            $(dropdownListSelector).hide();
        }
    });
}



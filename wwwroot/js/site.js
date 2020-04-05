"use strict";

$(() => {

    const $processTableBodyPlaceholder = $("#process-table-body-placeholder");
    const $modalPlaceholder = $("#modal-placeholder");
    const $spinner = $("i#spinner");

    function loadList() {
        toggleSpinner();
        $.get("/?handler=ProcessList").done((list) => {
            $processTableBodyPlaceholder.html(list);
        }).always(() => {
            toggleSpinner();
        });
    }

    function toggleSpinner() {
        $spinner.toggleClass("invisible");
    }

    function getProcessIdFromParents($button) {
        return parseInt($button.parents("tr").children("td.process-id").text());
    }

    loadList();
    setInterval(function() {
            loadList();
        },
        2000);

    $("button[data-toggle='sort-process-list']").on("click",
        function() {
            toggleSpinner();
            const index = parseInt($(this).parent().data("action-index"));
            const url = $(this).data("url");
            $.get(url, { "index": index }).done(() => {
                loadList();
            }).always(() => {
                toggleSpinner();
            });
        });

    $processTableBodyPlaceholder.on("click",
        "button[data-toggle='remove-process']",
        function() {
            toggleSpinner();
            const id = getProcessIdFromParents($(this));
            const url = $(this).data("url");
            $.get(url, { "id": id }).done(() => {
                loadList();
            }).always(() => {
                toggleSpinner();
            });
        });

    $processTableBodyPlaceholder.on("click",
        "button[data-toggle='open-folder']",
        function() {
            toggleSpinner();
            const id = getProcessIdFromParents($(this));
            const url = $(this).data("url");
            $.get(url, { "id": id }).done(() => {
                loadList();
            }).always(() => {
                toggleSpinner();
            });
        });
    
    $processTableBodyPlaceholder.on("click",
        "button[data-toggle='module-list-modal']",
        function() {
            toggleSpinner();
            const id = getProcessIdFromParents($(this));
            const url = $(this).data("url");
            $.get(url, { "id": id }).done((modal) => {
                $modalPlaceholder.html(modal);
                $modalPlaceholder.find(".modal").first().modal("show");
            }).always(() => {
                toggleSpinner();
            });
        });

    $processTableBodyPlaceholder.on("click",
        "button[data-toggle='thread-list-modal']",
        function() {
            toggleSpinner();
            const id = getProcessIdFromParents($(this));
            const url = $(this).data("url");
            $.get(url, { "id": id }).done((modal) => {
                $modalPlaceholder.html(modal);
                $modalPlaceholder.find(".modal").first().modal("show");
            }).always(() => {
                toggleSpinner();
            });
        });

});
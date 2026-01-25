var recId1 = "AC8A496F-DE70-4A86-A4D2-B71DE903524F";
var tempContentId = "";
var contentType = "";
//////// Filter Toggle
$(document).ready(function () {
    $('#collapse-filter-button').on('click', function () {
        $('#collapse-filter-container').stop(true, true).slideToggle(400);
    });
});

/////

$(document).on("click", ".dropdown-toggle", function () {
 
});

$(document).on("click", ".folder", function () {
    debugger
    if (!$(event.target).is('.folder .dropdown *')) {
        let clicked = $(this);
        let clickedTop = clicked.position().top;

        let all = $(".folder");
        let lastInRow = clicked;

        // find last element in same row
        all.each(function () {
            if ($(this).position().top === clickedTop) {
                lastInRow = $(this);
            }
        });
        recId1 = $(this).find(".dropdown-toggle").data("recid");

        const gridInstance = $('#addFilesGrid').dxDataGrid("instance");
        gridInstance.refresh();
      
        // move details under the row
        $("#sub-folders-details").insertAfter(lastInRow).slideDown();
    }
});
//////////////// Folder active
$(document).on("click", ".folder", function () {
    if (!$(event.target).is('.folder .dropdown *')) {
        $(".folder").removeClass("active");
        $(this).addClass("active");       
    }
});
//////////////// FAB Button
$(document).on("click", ".fab-btn",function () {
    let menu = $(".fab-menu");
    $(this).find("i").toggleClass("fa-edit fa-times");
    menu.toggleClass("show");

    $(".fab-item").each(function (i) {
        $(this).css("transition-delay", menu.hasClass("show") ? i * 100 + "ms" : "0ms");
    });
});
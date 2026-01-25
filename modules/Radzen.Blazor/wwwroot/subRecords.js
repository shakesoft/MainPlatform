
var mediaSubRecordsInfoGrid_dataSource = {
    key: "id",
    load: function (options) {
        var d = $.Deferred();
 
        var params = {};
        [
            "filter",
            "group",
            "groupSummary",
            "parentIds",
            "requireGroupCount",
            "requireTotalCount",
            "searchExpr",
            "searchOperation",
            "searchValue",
            "select",
            "sort",
            "skip",
            "take",
            "totalSummary",
            "userData"
        ].forEach(function (i) {
            if (i in options && (options[i] != null)) {
                params[i] = JSON.stringify(options[i]);
            }
        });

       
        return mediaManagement.subRecords.getAllSubRecord(params, parentRecId);

        
       
    },
    remove: function (key) {
        console.log("key", key)
        return mediaManagement.mediaRecordsInfo.deleteSubRecord(key);

    }
}

var parentRecId = getQueryParam("id");
function onAddFiles(id,title) {
   
    const parentIds = getQueryParam("id");
    var recId = id;
    var parentN = getQueryParam("recordName");
    var recordTitle = title;
    // URL-encode the recordTitle
    var encodedTitle = encodeURIComponent(recordTitle);
    var encodedParentName = encodeURIComponent(parentN);
    location.href = "/MediaManagement/AddFilesToSubRecord?id=" + recId + "&parentId=" + parentIds + "&recordName=" + encodedTitle + "&parentRecordName=" + encodedParentName;
}
function getQueryParam(param) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(param);
}
function ShowAttachments(e) {

    const parentId = getQueryParam("id");
    console.log("parentId", parentId);
    console.log("e", e);
    var recId = e.row.key;
    var parentN=getQueryParam("recordName");
    var recordTitle = e.row.data.recordName;
    // URL-encode the recordTitle
    var encodedTitle = encodeURIComponent(recordTitle);
    var encodedParentName = encodeURIComponent(parentN);
    location.href = "/MediaManagement/SubRecordAttachments?id=" + recId + "&parentId=" + parentId + "&recordName=" + encodedTitle + "&parentRecordName=" + encodedParentName;
}
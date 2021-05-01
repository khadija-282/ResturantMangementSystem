$(document).ready(function () {

    $("#Level").change(function () {
        populateRoles();

    });
    populateRoles();
});
function populateRoles() {
    if ($("#Level").val() != "") {

        var options = {};
        options.url = "/Users/GetRoles";
        options.type = "POST";
        options.data = JSON.stringify({ levelId: $("#Level").val(),Id:0 });
        options.dataType = "json";
        options.contentType = "application/json";
        options.success = function (roles) {

            $("#role").empty();
            $("#role").html("");
            $("#role").append('<option  value="0">-- Please Select Role --</option>');
            for (var i = 0; i < roles.length; i++) {

                $("#role").append('<option value="' + roles[i].Value + '">' + roles[i].Text + '</option>');

            }
            $("#role").show();
            $("#role").prop("disabled", false);

        };
        options.error = function (err) {
            alert("Error retrieving roles!");
        };
        $.ajax(options);
    }
    else {
        $("#role").empty();
        $("#role").append("<p> Please select User Level </p>");
        $("#role").prop("disabled", true);
    }
}
function showMenu(roleId) {

    $("#hfRoleID").val(roleId);
    
   $("#treeview").jstree("destroy");
   LoadTree();

}
function treeview_save() {
    menuIds = [];
    var checkedBoxes = $("#treeview").jstree("get_checked", null, true);
    
    for (var i = 0; i < checkedBoxes.length; i++) {
        menuIds.push(checkedBoxes[i]);
    }
    var checkedBoxes = $("#treeview").jstree("get_undetermined", null, true);
    for (var i = 0; i < checkedBoxes.length; i++) {
        menuIds.push(checkedBoxes[i]);
    }
    
    $.ajax({
        type: 'POST',
        url: '/RolePermission/Save',
        data: JSON.stringify({ MenuIds: menuIds, roleId: $("#hfRoleID").val() }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data)
        {            
            toastr.success("Permissions saved Successfully", "Success");
            return true;
        }

    });

    return true;
}
$(function () {
    var to = false;
    $('#demo_q').keyup(function () {
        if (to) { clearTimeout(to); }
        to = setTimeout(function () {
            var v = $('#demo_q').val();
            $('#treeview').jstree(true).search(v);
        }, 250);
    });
    LoadTree();
});
function LoadTree()
{ $('#treeview').jstree({

        'core': {
            "animation": 0,
            "check_callback": true,
            'force_text': true,
            "themes": { "stripes": true },
           
            'data': {
                'state': { 'opened': true, 'children': true },
                'url': function (node) {
                    return node.id === "#" ? "/TreeView/GetRootWithRole?roleId=" + $("#hfRoleID").val() : "/TreeView/GetChildren/" ;

                },
                'data': function (node) {
                    return { 'id': node.id };
                }
            }
        },
       
        "types": {
            "#": { "max_children": 1, "max_depth": 4, "valid_children": ["root"] },
            "root": { "icon": "/static/3.3.5/assets/images/tree_icon.png", "valid_children": ["default"] },
            "default": { "valid_children": ["default", "file"] },
            "file": { "icon": "glyphicon glyphicon-file", "valid_children": [] }
        },
        "checkbox": {
            "three_state": true
        },
        "plugins": ["search", "wholerow", "checkbox"]


    });}
$('#treeview').on('changed.jstree', function (e, data) {
   
    //if (data.node != null) {
        console.log("=> selected node: " + data.node.id);

        $("#hfSelectedNodeId").val(data.node.id);
        $("#hfSelectedNodeText").val(data.node.text);
   // }

});

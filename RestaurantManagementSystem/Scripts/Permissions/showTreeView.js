$('#treeview').on('rename_node.jstree', function (e, obj) {

    $.ajax({
        type: 'POST',
        url: '/Permissions/Edit',
        data: JSON.stringify({ MenuId: obj.node.id, newName: obj.node.text }),
        contentType: "application/json; charset=utf-8",
        dataType: "json"


    });
});

$('#treeview').on('delete_node.jstree', function (e, obj) {
    // alert(obj.node.id);
    $.ajax({
        type: 'POST',
        url: '/Permissions/Delete',
        data: JSON.stringify({ MenuId: $("#hfSelectedNodeId").val() }),
        contentType: "application/json; charset=utf-8",
        dataType: "json"

    });
});
$('#treeview').on('create_node.jstree', function (e, obj) {
   // alert(obj.node.text);
    var modal = { text: obj.node.text, icon: obj.node.text };
    $.ajax({
        type: 'POST',
        url: '/Permissions/Add',
        data: JSON.stringify({ p_MenuId: $("#hfSelectedNodeId").val(), Modal: modal }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $('#treeview').jstree(true).set_id(obj.node, data.id);
        }

    });
});

function treeview_create() {

    var ref = $('#treeview').jstree(true),
        sel = ref.get_selected();

   // alert(sel.length);
    if (!sel.length) {
        $("#treeview").jstree('create_node', '#', { 'id': 'myId', 'text': 'Root Node', type: "Root" }, 'last');
        //$("#treeview").jstree.create_node('#', { 'id': 'myId', 'text': 'Root Node', type: }, 'last');
    }
    sel = sel[0];
    sel = ref.create_node(sel, { "type": "file" });

    //$('#treeview').deselect_all(true);
    // $('#treeview').jstree(true).select_node(sel);
    //if (sel) {
    //    ref.edit(sel);
    //}

    //alert($("#hfSelectedNodeText").val());



};
function treeview_rename() {
    var ref = $('#treeview').jstree(true),
        sel = ref.get_selected();
    if (!sel.length) { return false; }
    sel = sel[0];
    ref.edit(sel);
};
function treeview_delete() {
    var x = confirm("Are you sure you want to delete this item?");
    if (x) {

        var ref = $('#treeview').jstree(true),
            sel = ref.get_selected();
        if (!sel.length) { return false; }
        ref.delete_node(sel);
    }
    else
        return false;
};
$(function () {
    var to = false;
    $('#demo_q').keyup(function () {
        if (to) { clearTimeout(to); }
        to = setTimeout(function () {
            var v = $('#demo_q').val();
            $('#treeview').jstree(true).search(v);
        }, 250);
    });
    $('#treeview').jstree({

        'core': {
            "animation": 0,
            "check_callback": true,
            'force_text': true,
            "themes": { "stripes": true },
            'data': {
                'state': { 'opened': true, 'children': true },
                'url': function (node) {
                    return node.id === "#" ? "/TreeView/GetRoot"  : "/TreeView/GetChildren/" + node.id;

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
        "plugins": ["search", "wholerow"]


    });
});

$('#treeview').on('changed.jstree', function (e, data) {
    console.log("=> selected node: " + data.node.id);

    $("#hfSelectedNodeId").val(data.node.id);
    $("#hfSelectedNodeText").val(data.node.text);


});

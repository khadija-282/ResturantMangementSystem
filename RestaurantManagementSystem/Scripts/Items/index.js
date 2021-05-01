$(document).ready(function () {

    $('#tblItems tfoot').toggle();
    // Setup - add a text input to each footer cell
    $('#tblItems tfoot th').each(function () {
        var title = $(this).text();
        $(this).html('<input type="text" id="filter_' + title + '"class="input-class" style="width:100px"/>');
    });

    //Datatables Configurations

    //var buttonCommon = {
    //    exportOptions: {
    //        format: {
    //            body: function (data, row, column, node) {
    //                return data;
    //            }
    //        }
    //    }
    //};
    var table = $("#tblItems").DataTable({
        'searchDelay': 200,
        "bServerSide": true,
        "sAjaxSource": "/Item/GetItems",
        "bProcessing": true,
        'paging': true,
        'lengthChange': true,
        'searching': true,
        'ordering': true,
        'info': true,
        'autoWidth': false,
        'responsive': true,
        "pageLength": 50,
        "lengthMenu": [[10, 25, 50, 100, 200], [10, 25, 50, 100, 200]],
        // scrollY: 400,
        //scroller: {
        //    loadingIndicator: true
        //},
        dom: 'Bflrtip',
        buttons: [
            {
                extend: 'excelHtml5',
                text: '<i class="fa fa-file-excel-o"></i>',
                className: 'btn',
                title: 'Items List',
                titleAttr: 'Export Excel',
                exportOptions: {
                    columns: [1, 2, 3, 5, 7,8]
                }
            },
            {
                extend: 'csvHtml5',
                text: '<i class="fa fa-file-text-o"></i>',
                className: 'btn',
                title: 'Items List',
                titleAttr: 'Export CSV',
                exportOptions: {
                    columns: [1, 2, 3, 5, 7, 8]
                }
            },
            {
                extend: 'pdfHtml5',
                text: '<i class="fa fa-file-pdf-o"></i>',
                className: 'btn',
                title: 'Items List',
                titleAttr: 'Export PDF',
                orientation: 'landscape',
                exportOptions: {
                    columns: [1, 2, 3, 5, 7, 8]
                }
            }
        ],
        "columnDefs": [
            { visible: false, searchable: false, targets: [0] },  
            {
                "targets": -1,
                "data": null,
                "render": function (data, type, full, meta) {
                    var editLink = ("1" == "1" ? '<a href="/Item/Edit/' + full[0] + '" class="fa fa-edit" ></a> &nbsp; &nbsp;' : '');
                    //var viewLink = ("1" == "1" ? '<a href="/Item/Details/' + full[0] + '" class="fa fa-eye" ></a> &nbsp; &nbsp;' : '');
                    var deleteLink = ("1" == "1" ? ' <a href="/Item/Delete/' + full[0] + '" class="fa fa-remove"></a> ' : '');
            
                    return editLink + deleteLink ;
                }

            }]

    });

    $('#btnAdvSearch').click(function () {
        $('#tblItems tfoot').toggle();
        $('#btnAdvSearch').text(function (_, txt) {
            var ret = '';
            if (txt == 'Advanced Search') {
                ret = 'Basic Search';
                $("[id^='filter_']").val("").change();
            } else {
                ret = 'Advanced Search';
                $("[id^='filter_']").val("").change();
            }
            return ret;
        });
        return false;
    });

    // Apply the search
    table.columns().every(function () {
        var that = this;
        $('input', this.footer()).on('keyup change', function () {
            if (that.search() !== this.value) {
                that.search(this.value).draw();
            }
        });
    });

    $("#filter_Actions").hide();
    $("#filter_Ser").hide();

});
    
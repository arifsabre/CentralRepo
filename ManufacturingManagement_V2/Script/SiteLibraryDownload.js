$(document).ready(function () {


    var groupColumn = 2;
    var table = $('#Library').DataTable({
        "columnDefs": [
            { "visible": false, "targets": groupColumn }
        ],
        "order": [[groupColumn, 'asc']],

        "scrollY": 250,
        "scrollX": true,
        "paging": false,
        // "pageLength":50,
        "stateSave": true,
        "order": [[0, "asc"]],
        //dom: 'lfBrtip',
        dom: 'Blfrtip',
        buttons: [
            { extend: 'colvis' },
            // { extend: 'copy' },
            { extend: 'excel' },
            { extend: 'pdf' },
            { extend: 'print' },
        ],

        "drawCallback": function (settings) {
            var api = this.api();
            var rows = api.rows({ page: 'current' }).nodes();
            var last = null;

            api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
                if (last !== group) {
                    $(rows).eq(i).before(
                        '<tr class="group"><td colspan="12">' + group + '</td></tr>'
                    );

                    last = group;
                }
            });
        }
    });
    $('#Library tbody').on('click', 'tr.group', function () {
        var currentOrder = table.order()[0];
        if (currentOrder[0] === groupColumn && currentOrder[1] === 'asc') {
            table.order([groupColumn, 'desc']).draw();
        }
        else {
            table.order([groupColumn, 'asc']).draw();
        }
    });
});

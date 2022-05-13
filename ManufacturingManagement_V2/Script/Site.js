//$(document).ready(function ()
//{
//    $('#jdtable').dataTable();
//});

$(document).ready(function () {
    var table = $('#jdtabledesktop').DataTable({
       
        "paging": false,
    
        "stateSave": true,
        "order": [[0, "asc"]],
        dom: 'lfBrtip',
        buttons: [
            { extend: 'excel'},
            { extend: 'pdf'},
            { extend: 'print'},
        ]
    });
    table.buttons().container().appendTo($("#printbar"));
});
  //dom: 'tlfBrip',
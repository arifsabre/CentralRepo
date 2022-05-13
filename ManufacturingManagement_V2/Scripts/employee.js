//Load Data in Table when documents is ready
$(document).ready(function () {
    loadData();
});
//Load Data function
function loadData() {
    $.ajax({
        url: "/BioMetric_Operation/List",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        //autostart:true,
        success: function (result) {
            var html = '';
            $.each(result, function (key, item) {
                html += '<tr>';
                html += '<td>' + item.RowId + '</td>';
                html += '<td>' + item.ECode + '</td>';
                html += '<td>' + item.EDate + '</td>';
                html += '<td>' + item.ETime + '</td>';
                html += '<td>' + item.MCNo + '</td>';
                html += '<td><a href="#" onclick="return getbyID(' + item.RowId + ')">Edit</a> | <a href="#" onclick="Delele(' + item.RowId + ')">Delete</a></td>';
                html += '</tr>';
            });
            $('.tbody').html(html);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//Add Data Function
function Add() {
    var res = validate();
    if (res == false) {
        return false;
    }
    var empObj = {
        RowId: $('#RowId').val(),
        ECode: $('#ECode').val(),
        EDate: $('#EDate').val(),
        ETime: $('#ETime').val(),
        MCNo: $('#MCNo').val()
    };
    $.ajax({
        url: "/BioMetric_Operation/Add",
        data: JSON.stringify(empObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            loadData();
            $('#myModal').modal('hide');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//Function for getting the Data Based upon Employee ID
function getbyID(RowId) {
    $('#RowId').css('border-color', 'lightgrey');
    $('#ECode').css('border-color', 'lightgrey');
    $('#EDate').css('border-color', 'lightgrey');
    $('#ETime').css('border-color', 'lightgrey');
    $('#MCNo').css('border-color', 'lightgrey');
    $.ajax({
        url: "/BioMetric_Operation/getbyID/" + RowId,
        typr: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            $('#RowId').val(result.RowId);
            $('#ECode').val(result.ECode);
            $('#EDate').val(result.EDate);
            $('#ETime').val(result.ETime);
            $('#MCNo').val(result.MCNo);
            $('#myModal').modal('show');
            $('#btnUpdate').show();
            $('#btnAdd').hide();
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
    return false;
}
//function for updating employee's record
function Update() {
    var res = validate();
    if (res == false) {
        return false;
    }
    var empObj = {
        RowId: $('#RowId').val(),
        ECode: $('#ECode').val(),
        EDate: $('#EDate').val(),
        ETime:$('#ETime').val,
        MCNo: $('#MCNo').val(),
        
    };
    $.ajax({
        url: "/BioMetric_Operation/Update",
        data: JSON.stringify(empObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            loadData();
            $('#myModal').modal('hide');
            $('#RowId').val("");
            $('#ECode').val("");
            $('#EDate').val("");
            $('#ETime').val("");
            $('#MCNo').val("");
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}
//function for deleting employee's record
function Delele(RowId) {
    var ans = confirm("Are you sure you want to delete this Record?");
    if (ans) {
        $.ajax({
            url: "/BioMetric_Operation/Delete/" + RowId,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                loadData();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }
}
//Function for clearing the textboxes
function clearTextBox() {
    $('#RowId').val("");
    $('#ECode').val("");
    $('#EDate').val("");
    $('#ETime').val("");
    $('#MCNo').val("");
    $('#btnUpdate').hide();
    $('#btnAdd').show();
    $('#RowId').css('border-color', 'lightgrey');
    $('#ECode').css('border-color', 'lightgrey');
    $('#ETime').css('border-color', 'lightgrey');
    $('#EDate').css('border-color', 'lightgrey');
    $('#MCNo').css('border-color', 'lightgrey');
}
//Valdidation using jquery
function validate() {
    var isValid = true;
    if ($('#RowId').val().trim() == "") {
        $('#RowId').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#RowId').css('border-color', 'lightgrey');
    }
    if ($('#ECode').val().trim() == "") {
        $('#ECode').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#ECode').css('border-color', 'lightgrey');
    }
    if ($('#EDate').val().trim() == "") {
        $('#EDate').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#EDAte').css('border-color', 'lightgrey');
    }
    if ($('#ETime').val().trim() == "") {
        $('#ETime').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#ETime').css('border-color', 'lightgrey');
    }

    if ($('#MCNo').val().trim() == "") {
        $('#MCNo').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#MCNo').css('border-color', 'lightgrey');
    }
    return isValid;
}
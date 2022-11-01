// Datables Need to be re-drawn again if datatable intialised in hidden area(That is when we put datatables inside tabs)
// So here we are call draw to redraw table when each tabs become visible
$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    var table = $('table.bmdatatable').DataTable();
    table.columns.adjust().draw();
});

//Row double click edit fuction - Datatble
$(document).on("dblclick", 'table.bmdatatable.dblEdit tr', function () {

 
    var unid = $(this).attr('data-unid');
    $(this).addClass("highlight");
    if (jQuery.type(unid) !== "undefined") {
        //Forms in 1st Column
        var Forms = $(this).find("td:nth-child(1) form");
        if (Forms.length>0) {
            $(this).find("td:nth-child(1) form").find("input[id$='ID'][type = hidden]").val(unid);
            $(this).find("td:nth-child(1) form").submit();
        }
        else {
            //Forms in 2nd Column
            var Forms2ndColumn = $(this).find("td:nth-child(2) form");
            if (Forms2ndColumn.length > 0) {
                $(this).find("td:nth-child(2) form").find("input[id$='ID'][type = hidden]").val(unid);
                $(this).find("td:nth-child(2) form").submit();
            }
        }
    }
    else {
         
        $(this).find("td:nth-child(1)").find("form#DetailForm").submit();
    }
});

//Row edit fuction DataTable
$(document).on("click", 'table.bmdatatable #EditRow', function () {
    var unid = $(this).attr('data-unid');
    
    if (jQuery.type(unid) !== "undefined") {
        $(this).parent('form').find("input[id$='ID'][type = hidden]").val(unid);
        $(this).parent('form').submit();
       
        
    }
});

//Row edit fuction DataTable
$(document).on("click", 'table.bmdatatable #DeleteRow', function () {
    var unid = $(this).attr('data-unid');
    if (jQuery.type(unid) !== "undefined") {

        if ($(this).parent('form').hasClass('active')) {
            //$(this).parent('form').removeClass('active');
        }
        else {

            //set Form Active
            var table = $(this).closest('table');
            table.find('tr form.active').removeClass('active');
            $(this).parent('form').addClass('active');
            $(this).parent('form').attr("data-formid", unid);

            //Set Hidden UID
            $(this).parent('form').find("input[id$='ID'][type = hidden]").val(unid);

            $('#DeleteConfirmationModal').find('#deleteLine').attr("data-formid", unid);
        }
    }
});

$('body').on("click", '#deleteLine', function () {
    var formId = $(this).attr("data-formid");

    //var foundForm = $('div#UsrListTble').find("form[data-formid='" + formId + "']");
    var foundForm = $(document).find("form.active");
    if (foundForm.length >= 1) {
        var formuniid = $(foundForm).attr("data-formid");
        if (formId == formuniid) {
            $('#DeleteConfirmationModal').modal('toggle');
            $(foundForm).submit();
        }
    }
    else {
        alert('form not found');
    }
});

// Modal Openened using Button Click
$(document).on('show.bs.modal', '.modalform', function (event) { 
  
    if (typeof event.relatedTarget !== "undefined") {        
        if (event.relatedTarget.nodeName === "BUTTON") {          
            $(this).find('form').trigger("reset");
            $(this).find('form').find("input[id$='ID']").val('0');
            $(this).find('form').find("input[type=text]").val('');
            $(this).find('form').find("input[type=checkbox]").prop('checked', false);

            $(this).find('form .validation-summary-errors ul li').remove(); // Remove All li if before Submission had client side errors
            $(this).find('form .validation-summary-valid ul li').remove(); 

        }
    }
});

$(document).ready(function () {     
    //Initialize Select2 Elements
   

    $('#GlobalSuccessMsg,#GlobalErrMsg').delay(5000).slideUpAndRemove('slow');
    

    $('[data-toggle="tooltip"]').tooltip();

    $('table.bmdatatable').DataTable(
        {
            //"oLanguage": {
            //    "sLengthMenu": "Show _MENU_ Stock",

            //}  
            "order": [[0, "desc"]]
        }
    );
    $('body').popover({
        selector: '[data-toggle="popover"]',
        trigger: 'hover',
        html: true,
    });
});
function deleteConfirmation() {
    $('#DeleteConfirmationModal').modal('toggle');
    return false;
}
function AjaxSimpleFormSuccessNoReplace(FormContainer, FormID, Message, data) {

    if (Boolean(data.IsOpertationSuccess) === true) {
       

            if (jQuery.type(FormContainer) !== "") {
                //$("#" + data.Ax.EditActionTargetId).prepend('<div class="alert alert-success alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i> Success!</h4>Added SuccessFully</div>');
                $('<div class="alert alert-success alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i> Success!</h4>' + Message + '</div>')
                    .prependTo($("#" + FormContainer))
                    .delay(5000)
                    .slideUpAndRemove('slow');
                
            }
        
    }
    else {
        $("#" + FormID + " .validation-summary-errors ul li").remove(); // Remove All li if before Submission had client side errors
        $("#" + FormID + " .validation-summary-valid ul li").remove();  // Remove one li with diplay:none style if before Submission didn't had any client side errors

        for (var i = 0; i < data.ErrList.length; i++) {
            $("#" + FormID + " .validation-summary-errors ul").append("<li>" + ErrArray[i] + "</li>");
            $("#" + FormID + " .validation-summary-valid ul").append("<li>" + ErrArray[i] + "</li>");
        }
    }

}
function AjaxFormSuccessModal(UpdateTargetId, FormID, ModalID, data) {
 
    if (Boolean(data.IsOpertationSuccess) === true) {


        if (Boolean(data.IsResultDataTableOpertation) === true) {

            if (parseInt(data.Opertation) === 1) {

                var Tbl = $('#' + UpdateTargetId).DataTable();
                Tbl.row.add(data.rowValues).draw(false);

            }
        }
        else {

           
            if (parseInt(data.Opertation) === 1) { // Add Operation              

                $("#" + UpdateTargetId).html(data.ContentToUpdateorReplace);
                var Tbl = $('#_' + UpdateTargetId).DataTable();
                Tbl.draw();

                $("#" + FormID).trigger("reset"); // clears form inputs not hidden field values
                $("#" + FormID + " .validation-summary-errors ul li").remove(); // Remove All li if before Submission had client side errors
                $("#" + FormID + " .validation-summary-valid ul li").remove(); // Remove one li with diplay:none style if before Submission didn't had any client side errors
                $("#" + FormID).find("input[type=text]").val('');

                if ($("#" + FormID).closest(".modalform") !== "undefined") {
                    var Modal = $("#" + FormID).closest(".modalform");
                    $('<div class="alert alert-success alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i> Success!</h4>Added Successfully</div>')
                        .prependTo($(Modal).find(".modal-body"))
                        .delay(5000)
                        .slideUpAndRemove('slow');
                }
            }
            else if (parseInt(data.Opertation) === 2) { // Edit
               
                if ($("#" + ModalID) !== "undefined") {
                    $("#" + ModalID).replaceWith(data.ContentToUpdateorReplace);
                    $("#" + ModalID).modal("show");
                }
            }
            else if (parseInt(data.Opertation) === 3) {

                $("#" + UpdateTargetId).html(data.ContentToUpdateorReplace);
                var Tbl = $('#_' + UpdateTargetId).DataTable();
                Tbl.draw();

                $('<div class="alert alert-success alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i> Success!</h4>Deleted Successfully</div>')
                    .prependTo($("#" + UpdateTargetId))
                    .delay(5000)
                    .slideUpAndRemove('slow');
            }
            else if (parseInt(data.Opertation) === 4) {
                $("#" + UpdateTargetId).html(data.ContentToUpdateorReplace);
                var Tbl = $('#_' + UpdateTargetId).DataTable();
                Tbl.draw();

                $("#" + FormID).trigger("reset"); // clears form inputs not hidden field values
                $("#" + FormID + " .validation-summary-errors ul li").remove(); // Remove All li if before Submission had client side errors
                $("#" + FormID + " .validation-summary-valid ul li").remove(); // Remove one li with diplay:none style if before Submission didn't had any client side errors

                if (jQuery.type(data.Ax.EditActionTargetId) !== "undefined") {
                    //$("#" + data.Ax.EditActionTargetId).prepend('<div class="alert alert-success alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i> Success!</h4>Added SuccessFully</div>');
                    $('<div class="alert alert-success alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i> Success!</h4>Updated Successfully</div>')
                        .prependTo($("#" + data.Ax.EditActionTargetId))
                        .delay(5000)
                        .slideUpAndRemove('slow');
                }
                 
                $("#" + ModalID).modal("hide");
                }
            }


        }    
    else {
        var ErrArray = data.ErrList;
        if (parseInt(data.Opertation) === 1) { // Add Operation        
            $("#" + FormID + " .validation-summary-errors ul li").remove(); // Remove All li if before Submission had client side errors
            $("#" + FormID + " .validation-summary-valid ul li").remove();  // Remove one li with diplay:none style if before Submission didn't had any client side errors

            for (var i = 0; i < data.ErrList.length; i++) {
                $("#" + FormID + " .validation-summary-errors ul").append("<li>" + ErrArray[i] + "</li>");
                $("#" + FormID + " .validation-summary-valid ul").append("<li>" + ErrArray[i] + "</li>");
            }
        }
        else if (parseInt(data.Opertation) === 2) { // Edit Operation 
            
            if (jQuery.type(data.Ax.AddActionTargetId) !== "undefined") {             
                //$("#" + data.Ax.AddActionTargetId).html(data.ContentToUpdateorReplace);
                //var Tbl = $('#_' + data.Ax.AddActionTargetId).DataTable();
                //Tbl.draw();

                //$("#" + data.Ax.AddActionTargetId + " .validation-summary-errors ul li").remove(); // Remove All li if before Submission had client side errors
                //$("#" + data.Ax.AddActionTargetId + " .validation-summary-valid ul li").remove();  // Remove one li with diplay:none style if before Submission didn't had any client side errors

                //for (var i = 0; i < data.ErrList.length; i++) {
                //    $("#" + data.Ax.AddActionTargetId + " .validation-summary-errors ul").append("<li>" + ErrArray[i] + "</li>");
                //    $("#" + data.Ax.AddActionTargetId + " .validation-summary-valid ul").append("<li>" + ErrArray[i] + "</li>");
                //}

                var ul = $('<ul>');
                for (var i = 0; i < data.ErrList.length; i++) {
                    $(ul).append("<li>" + ErrArray[i] + "</li>");
                }

                if (jQuery.type(data.Ax.EditActionTargetId) !== "undefined") {
                    //$("#" + data.Ax.EditActionTargetId).prepend('<div class="alert alert-success alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i> Success!</h4>Added SuccessFully</div>');
                    $('<div class="alert alert-danger alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-ban"></i> Alert!</h4>' + ul.html() + '</div>')
                        .prependTo($("#" + data.Ax.EditActionTargetId))
                        .delay(5000)
                        .slideUpAndRemove('slow');
                }
            }
        }
        else if (parseInt(data.Opertation) === 3) { // Delete Operation                

            var ul = $('<ul>');
            for (var i = 0; i < data.ErrList.length; i++) {
                $(ul).append("<li>" + ErrArray[i] + "</li>");
            }

            if (jQuery.type(data.Ax.EditActionTargetId) !== "undefined") {
                //$("#" + data.Ax.EditActionTargetId).prepend('<div class="alert alert-success alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i> Success!</h4>Added SuccessFully</div>');
                $('<div class="alert alert-danger alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-ban"></i> Alert!</h4>' + ul.html() + '</div>')
                    .prependTo($("#" + data.Ax.EditActionTargetId))
                    .delay(5000)
                    .slideUpAndRemove('slow');
            }
            
        }
        else if (parseInt(data.Opertation) === 4) { // Update Operation   

            $("#" + FormID + " .validation-summary-errors ul li").remove(); // Remove All li if before Submission had client side errors
            $("#" + FormID + " .validation-summary-valid ul li").remove();  // Remove one li with diplay:none style if before Submission didn't had any client side errors

            for (var i = 0; i < data.ErrList.length; i++) {
                $("#" + FormID + " .validation-summary-errors ul").append("<li>" + ErrArray[i] + "</li>");
                $("#" + FormID + " .validation-summary-valid ul").append("<li>" + ErrArray[i] + "</li>");
            }
        }

    }
}
function AjaxFormSuccess(UpdateTargetId, FormID, data) {

    if (Boolean(data.IsOpertationSuccess) === true) {

       
        if (Boolean(data.IsResultDataTableOpertation) === true) {
           
            if (parseInt(data.Opertation) === 1) {

                var Tbl = $('#' + UpdateTargetId).DataTable();
                Tbl.row.add(data.rowValues).draw(false);

            }
        }
        else {
           

            if (parseInt(data.Opertation) === 1) { // Add Operation              

                $("#" + UpdateTargetId).html(data.ContentToUpdateorReplace);
                var Tbl = $('#_' + UpdateTargetId).DataTable();
                Tbl.draw();
                $('.select2').select2(); // for User edit screen this has been added
                $("#" + FormID).trigger("reset"); // clears form inputs not hidden field values
                $("#" + FormID + " .validation-summary-errors ul li").remove(); // Remove All li if before Submission had client side errors
                $("#" + FormID + " .validation-summary-valid ul li").remove(); // Remove one li with diplay:none style if before Submission didn't had any client side errors

                if (jQuery.type(data.Ax.EditActionTargetId) !== "undefined") {
                    //$("#" + data.Ax.EditActionTargetId).prepend('<div class="alert alert-success alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i> Success!</h4>Added SuccessFully</div>');
                    $('<div class="alert alert-success alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i> Success!</h4>Added Successfully</div>')
                        .prependTo($("#" + data.Ax.EditActionTargetId))
                        .delay(5000)
                        .slideUpAndRemove('slow');
                }
            }
            else if (parseInt(data.Opertation) === 2) {
                if (jQuery.type(data.Ax.EditActionTargetId) !== "undefined") {

                    $("#" + data.Ax.EditActionTargetId).html(data.ContentToUpdateorReplace);
                    $('.select2').select2(); // for User edit screen this has been added
                }
            }
            else if (parseInt(data.Opertation) === 3) {

                $("#" + UpdateTargetId).html(data.ContentToUpdateorReplace);
                var Tbl = $('#_' + UpdateTargetId).DataTable();
                Tbl.draw();
                $('.select2').select2(); // for User edit screen this has been added
                $('<div class="alert alert-success alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i> Success!</h4>Deleted Successfully</div>')
                    .prependTo($("#" + UpdateTargetId))
                    .delay(5000)
                    .slideUpAndRemove('slow');
            }
            else if (parseInt(data.Opertation) === 4) {
                $("#" + UpdateTargetId).html(data.ContentToUpdateorReplace);
                var Tbl = $('#_' + UpdateTargetId).DataTable();
                Tbl.draw();
                $('.select2').select2(); // for User edit screen this has been added
                if (jQuery.type(data.Ax.EditActionTargetId) !== "undefined") {
                    $("#" + data.Ax.EditActionTargetId).html(data.ContentToLoadNewForm);
                    $("#" + data.Ax.EditActionTargetId).find('form').find("input[type=text], textarea").val(""); // clear form input field values
                    $("#" + data.Ax.EditActionTargetId).find('form').find("input[type=hidden][name!='__RequestVerificationToken']").val(""); // clear form hidden field values except validation tokent field

                    var AddForm = $("#" + data.Ax.EditActionTargetId).find('form');
                    jQuery.validator.unobtrusive.parse(AddForm); 
                    $('<div class="alert alert-success alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button><h4><i class="icon fa fa-check"></i> Success!</h4>Updated Successfully</div>')
                        .prependTo($("#" + data.Ax.EditActionTargetId))
                        .delay(5000)
                        .slideUpAndRemove('slow');

                }
            }
            

        }
    }
    else {
         
        var ErrArray = data.ErrList;
        if (parseInt(data.Opertation) === 1) { // Add Operation        
            $("#" + FormID + " .validation-summary-errors ul li").remove(); // Remove All li if before Submission had client side errors
            $("#" + FormID + " .validation-summary-valid ul li").remove();  // Remove one li with diplay:none style if before Submission didn't had any client side errors
           
            for (var i = 0; i < data.ErrList.length; i++) {
                $("#" + FormID + " .validation-summary-errors ul").append("<li>" + ErrArray[i] + "</li>");
                $("#" + FormID + " .validation-summary-valid ul").append("<li>" + ErrArray[i] + "</li>");
            }
        }
        else if (parseInt(data.Opertation) === 2) { // Update Operation 
            if (jQuery.type(data.Ax.AddActionTargetId) !== "undefined") {
               
                $("#" + data.Ax.AddActionTargetId).html(data.ContentToUpdateorReplace);
                var Tbl = $('#_' + data.Ax.AddActionTargetId).DataTable();
                Tbl.draw();

                $("#" + data.Ax.AddActionTargetId + " .validation-summary-errors ul li").remove(); // Remove All li if before Submission had client side errors
                $("#" + data.Ax.AddActionTargetId + " .validation-summary-valid ul li").remove();  // Remove one li with diplay:none style if before Submission didn't had any client side errors

                for (var i = 0; i < data.ErrList.length; i++) {
                    $("#" + data.Ax.AddActionTargetId + " .validation-summary-errors ul").append("<li>" + ErrArray[i] + "</li>");
                    $("#" + data.Ax.AddActionTargetId + " .validation-summary-valid ul").append("<li>" + ErrArray[i] + "</li>");
                }
            }            
        }
        else if (parseInt(data.Opertation) === 3 ) { // Delete Operation       
           
            $("#" + UpdateTargetId).html(data.ContentToUpdateorReplace);
            var Tbl = $('#_' + UpdateTargetId).DataTable();
            Tbl.draw();

            $("#" + UpdateTargetId + " .validation-summary-errors ul li").remove(); // Remove All li if before Submission had client side errors
            $("#" + UpdateTargetId + " .validation-summary-valid ul li").remove();  // Remove one li with diplay:none style if before Submission didn't had any client side errors

            for (var i = 0; i < data.ErrList.length; i++) {
                $("#" + UpdateTargetId + " .validation-summary-errors ul").append("<li>" + ErrArray[i] + "</li>");
                $("#" + UpdateTargetId + " .validation-summary-valid ul").append("<li>" + ErrArray[i] + "</li>");
            }
        }
        else if (parseInt(data.Opertation) === 4) { // Delete Operation   
            $("#" + FormID + " .validation-summary-errors ul li").remove(); // Remove All li if before Submission had client side errors
            $("#" + FormID + " .validation-summary-valid ul li").remove();  // Remove one li with diplay:none style if before Submission didn't had any client side errors

            for (var i = 0; i < data.ErrList.length; i++) {
                $("#" + FormID + " .validation-summary-errors ul").append("<li>" + ErrArray[i] + "</li>");
                $("#" + FormID + " .validation-summary-valid ul").append("<li>" + ErrArray[i] + "</li>");
            }
        }

    }
}


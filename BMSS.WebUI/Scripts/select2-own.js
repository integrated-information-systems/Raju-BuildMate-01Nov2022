$(function () {
    $('.select2.trigger').select2();   
});
$('.select2.trigger').on('select2:select', function (e) {
    var selectedText = $(this).find("option:selected").text();
    var selectedValue = $(this).find("option:selected").val();

    var trigger_target = $(this).attr('data-trigger-target');
    var trigger_by = $(this).attr('data-trigger-by');

    if (jQuery.type(trigger_target) !== "undefined") {
        if (jQuery.type(trigger_by) !== "undefined") {
            if (trigger_by == "Value") {
                $('#' + trigger_target).val(selectedValue).trigger('change');
            }
            else {
                $('#' + trigger_target).val(selectedText).trigger('change');
            }
        }
    }
    var DetailsURL = $(this).attr('data-getdet-url');
    var DetailsURLParamArray = $(this).data('getdet-param'); // if we use attribute() to retrive the value result will be a string not an object


    
    if (jQuery.type(DetailsURL) !== "undefined") {        
        if (jQuery.type(DetailsURLParamArray) !== "undefined") {

            var AjaxParamJsonArray = {};
            for (var key in DetailsURLParamArray) {
                if (DetailsURLParamArray.hasOwnProperty(key)) {                         
                    AjaxParamJsonArray[DetailsURLParamArray[key]] = $('#'+DetailsURLParamArray[key]).val();
                }
            }
        
            $.ajax({
            type: "POST",
            url: DetailsURL,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(AjaxParamJsonArray),
            success: function (data) {
                for (var key in data) {
                    $("#" + key).val(data[key]);
                    $("#" + key + ".select2").val(data[key]).trigger('change');
                }
            },
            error: function () { /*alert('Success');*/ }
            });
        }
    }

});
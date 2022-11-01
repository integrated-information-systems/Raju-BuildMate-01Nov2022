//Nav Tabs will work only if we include this
$('.nav-tabs .nav-item').click(function () {   
    $('.nav-tabs .nav-item').removeClass('active');
    $(this).addClass('active');
});

jQuery.fn.fadeOutAndRemove = function (speed) {
    $(this).fadeOut(speed, function () {
        $(this).remove();
    });
};

jQuery.fn.slideUpAndRemove = function (speed) {
    $(this).slideUp(speed).promise().done(function () {
        $(this).remove();
    });
};
function numberWithCommas(n, numberofdecimal) {    
    var formatted_n = parseFloat(n).toFixed(numberofdecimal);
    var parts = formatted_n.toString().split(".");
    return parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",") + (parts[1] ? "." + parts[1] : "");
}
function roundToTwo(num) {    
    if (typeof num !== 'number') {
        return num;
    }
    else {
        //return +(Math.round(num + "e+2") + "e-2");
        let formatted = Math.round((num + Number.EPSILON) * 100) / 100;        
        if (isInt(formatted)) {
            return parseFloat(formatted).toFixed(2);
        }
        else {
            return parseFloat(formatted).toFixed(2);
        }
    }
        
}
function isInt(n) {
    return n % 1 === 0;
}
let groupBy = function (xs, key) {
    return xs.reduce(function (rv, x) {
        (rv[x[key]] = rv[x[key]] || []).push(x);
        return rv;
    }, {});
};
//(function ($) {

//    var $jQval = $.validator;

//    $jQval.addMethod("requirediftrue",
//        function (value, element, parameters) {
//            return value !== "" && value != null;
//        }
//    );

//    var adapters = $jQval.unobtrusive.adapters;
//    adapters.addBool('requirediftrue');

//})(jQuery);

$('#StatusID').bind('change', function () {
    $('#StatusNaziv').val(this.options[this.selectedIndex].text);
});
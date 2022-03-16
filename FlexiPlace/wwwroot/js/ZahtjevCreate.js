let formSubmitted = false;

$(document).ready(function () {
    $.ajaxSetup({ cache: false });

    $("#DatumOdsustva").change(function () {
        $("#DatumOdsustva").valid();
    });

    $("#VrijemeOdsustvaOd").change(function () {
        $("#VrijemeOdsustvaOd").valid();
    });

    $("#DatumOdsustva").change();

    $('#VrijemeOdsustvaOd').change();

    $('#VrijemeOdsustvaOd').on('input', function () {
        $.ajax({
            type: "POST",
            url: "/Zahtjev/ComputeVrijemeOdsustvaDo",
            data: { "VrijemeOdsustvaOd": $("#VrijemeOdsustvaOd").val() },
            success: function (response) {
                console.log(response.vrijemeOdsustvaDo)
                if (!response.poruka) {
                    $('#VrijemeOdsustvaDo').val(response.vrijemeOdsustvaDo);
                }
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    });
});

$("form").submit(function () {
    if (!$(this).valid() || $(".validation-summary-errors").length || $.active > 0) {
        console.log($.active);
        return false;
    }
    else {
        console.log($.active);
        formSubmitted = true;
        return true;
    }
});
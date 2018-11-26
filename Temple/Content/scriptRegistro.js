
// Toolbar extra buttons
var btnFinish = $('<button></button>')
    .text('Presentar')
    .addClass('btn btn-info')
    .on(
        'click',
        function () {

            if (!$(this).hasClass('disabled')) {
                var elmForm = $("#myForm");
                if (elmForm) {
                    elmForm
                        .validator('validate');
                    var elmErr = elmForm
                        .find('.has-error');
                    if (elmErr
                        && elmErr.length > 0) {
                        alert('Oops we still have error in the form');
                        return false;
                    } else {
                        alert('Great! we are ready to submit form');
                        elmForm.submit();
                        return false;
                    }
                }
            }
        });
var btnCancel = $('<button></button>').text('Cancelar')
    .addClass('btn btn-danger').on(
        'click',
        function () {

            $('#smartwizard').smartWizard(
                "reset");
            $('#myForm')
                .find("input, textarea")
                .val("");
        });

// Smart Wizard
$('#smartwizard').smartWizard({
    selected: 0,
    theme: 'arrows',
    transitionEffect: 'fade',
    toolbarSettings: {
        toolbarPosition: 'bottom',
        toolbarExtraButtons: [
            btnFinish, btnCancel],
        cycleSteps: false, // Allows to
        // cycle the
        // navigation of
        // steps
        backButtonSupport: true
        // Enable the back button support

    },
    anchorSettings: {
        markDoneStep: true, // add done
        // css
        markAllPreviousStepsAsDone: true, // When
        // a
        // step
        // selected
        // by
        // url
        // hash,
        // all
        // previous
        // steps
        // are
        // marked
        // done
        removeDoneStepOnNavigateBack: true, // While
        // navigate
        // back
        // done
        // step
        // after
        // active
        // step
        // will
        // be
        // cleared
        enableAnchorOnDoneStep: true
        // Enable/Disable the done steps
        // navigation
    },
    lang: { // Language variables
        next: 'Continuar',
        previous: 'Retroceder'
    },

    toolbarSettings: {
        toolbarPosition: 'bottom', // none,
        // top,
        // bottom,
        // both
        toolbarButtonPosition: 'right', // left,
        // right
        showNextButton: true, // show/hide
        // a Next
        // button
        showPreviousButton: true, // show/hide
        // a
        // Previous
        // button
        toolbarExtraButtons: [
            $('<button></button>').text('Presentar').addClass('btn btn-info btn-finish d-none')
                .on('click', function () {

                    //Si acepta los términos y condiciones
                    var acepta = $(
                        "#chkAcepta")
                        .is(
                            ":checked");

                    if (acepta == false) {

                        alert("Debe aceptar los términos y condiciones");

                    } else {

                        // Verifico que no hayan campos vacíos
                        if ($("#txtNombres").val() != '' &&
                            $("#txtApPat").val() != '' &&
                            $("#txtApMat").val() != '' &&
                            $("#txtEdad").val() != '' &&
                            $("#txtCorreo").val() != '' &&
                            $("#txtTelefono").val() != '' &&
                            $("#txtUsuario").val() != '' &&
                            $("#txtClave").val() != '' &&
                            $("#txtSobreMi").val() != '' &&
                            $("#txtBuscando").val() != '') {

                            //Numero de preferencias registradas, se van a recorrer
                            var preferencias = $(".fieldPreferencia");

                            //Guarda los registros de experiencias
                            var arreglo = [];

                            for (var i = 0; i < preferencias.length; i++) {

                                var e = new Object();

                                e.idCat = $($(".selectPreferenciaCategoria").get(i)).val();
                                e.idSub = $($(".selectPreferenciaSubcategoria").get(i)).val();
                                //alert(e.idCat + " " + e.idSub);

                                arreglo[i] = e;

                            }

                            $("#preferencias").val(JSON.stringify(arreglo));
                            
                            document.forms[0].submit;


                        } else {

                            alert("Hay campos sin llenar");


                        }

                        

                    }






                }),


            $('<button></button>').text('Cancelar').addClass('btn btn-danger')
                .on('click', function () {

                })
        ]
    },

    anchorSettings: {
        anchorClickable: true, // Enable/Disable
        // anchor
        // navigation
        enableAllAnchors: true, // Activates
        // all
        // anchors
        // clickable
        // all
        // times
        markDoneStep: true, // add done
        // css
        enableAnchorOnDoneStep: true
        // Enable/Disable the done steps
        // navigation
    },
    contentURL: null, // content url,
    // Enables Ajax
    // content loading.
    // can set as data
    // data-content-url
    // on anchor
    disabledSteps: [], // Array Steps
    // disabled
    errorSteps: [], // Highlight step
    // with errors
    transitionSpeed: '400'

});

$("#smartwizard")
    .on(
        "leaveStep",
        function (e, anchorObject, stepNumber,
            stepDirection) {

            var elmForm = $("#form-step-"
                + stepNumber);
            // stepDirection === 'forward' :- this
            // condition allows to do the form
            // validation
            // only on forward navigation, that
            // makes easy navigation on backwards
            // still do the validation when going
            // next
            if (stepDirection === 'forward'
                && elmForm) {
                elmForm.validator('validate');
                var elmErr = elmForm
                    .children('.has-error');

                if (elmErr && elmErr.length > 0) {
                    alert("Debe rellenar todos los campos primero");

                    return false;

                }

            }

            return true;
        });

$("#smartwizard").on("showStep", function (e, anchorObject, stepNumber, stepDirection) {
    // Enable finish button only on last
    // step
    if (stepNumber == 3) {
        $('.btn-finish').removeClass('d-none');
    } else {
        $('.btn-finish').addClass('d-none');
    }
});

// Para agregar los cuadros de preferencia

$('#btnPreferencia')
    .click(
        function () {

            var first = $('.fieldPreferencia')
                .first();

            var last = $('.fieldPreferencia')
                .last();

            var cloned = $(first).clone(true);

            cloned.css('opacity', '0.1');
            cloned.insertAfter($(last));
            $(cloned)
                .slideDown(
                    150,
                    function () {
                        $(cloned)
                            .fadeTo(
                                500,
                                1);
                    });
            // Agrego boton cerrar

            var last = $('.fieldPreferencia').last();

            $(last).prepend('<button type="button" class="close cerrarPreferencia" aria-label="Close">   <span aria-hidden="true">&times;</span> </button>');
            $(last).find('legend').text("¿Qué más te gustaría aprender?");

            $(last).on('click', 'button.cerrarPreferencia', function () {

                $(last).fadeTo(
                    500,
                    0.01,
                    function () {
                        $(
                            last)
                            .slideUp(
                                150,
                                function () {
                                    $(
                                        last)
                                        .remove();
                                });
                    });
                if ($('.fieldPreferencia').length <= 3) {
                    $(
                        '#btnPreferencia')
                        .prop(
                            'disabled',
                            false);

                }

            });

            // Verifico si excedio el limite

            if ($('.fieldPreferencia').length >= 3) {

                $(this).prop('disabled', true);

            }

        });
// Smart Wizard
$('#smartwizard').smartWizard({
    selected: 0,
    theme: 'arrows',
    transitionEffect: 'fade',
    keyNavigation: false,
    toolbarSettings: {
        toolbarPosition: 'bottom',
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
        removeDoneStepOnNavigateBack: false, // While
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

// Siempre va a iniciar en el paso 1, así se actualice la página
$('#smartwizard').smartWizard("reset");

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
    
    // Para mostrar botón de enviar solo en el paso 4
    if (stepNumber == 2) {
        //map = null;
        window.dispatchEvent(new Event('resize'));
        $('#mapid').show();
        cargarMapa();
    }
    if (stepNumber == 3) {

        $('#btnSubmit').show();

    } else {

        $('#btnSubmit').hide();

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
            $(last).find('legend').text("¿Qué más te gustaría enseñar?");

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
                if ($('.fieldPreferencia').length <= 5) {
                    $(
                        '#btnPreferencia')
                        .prop(
                            'disabled',
                            false);

                }

            });

            // Verifico si excedio el limite

            if ($('.fieldPreferencia').length >= 5) {

                $(this).prop('disabled', true);

            }

        });

$(".btn-toolbar").append('<div class="btn-group mr-2" role="group"><button id="btnSubmit" class="btn btn-info" style="display:none;">Presentar</button><button id="btnCancelar" class="btn btn-danger">Cancelar</button></div>');
$("#btnSubmit").on('click', function () {

    //Si acepta los términos y condiciones
    var acepta = $(
        "#chkAcepta")
        .is(
            ":checked");

    if (!acepta) {

        alert("Debe aceptar los términos y condiciones");

    }

    else {

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
            $("#txtEspecialidad").val() != '' &&
            $('#flImagen').val() != '') {

            //Numero de preferencias registradas, se van a recorrer
            var preferencias = $(".fieldPreferencia");

            //Guarda los registros de experiencias
            var arreglo = [];

            for (var i = 0; i < preferencias.length; i++) {

                var e = new Object();

                e.idCat = $($(".selectPreferenciaCategoria").get(i)).val();
                e.idSub = $($(".selectPreferenciaSubcategoria").get(i)).val();
                e.descripcion = $($(".txtDescripcion").get(i)).val();
                e.silabo = $($(".txtSilabo").get(i)).val();

                var modalidades = $($(".fieldPreferencia").get(i)).find(".chkIdMod");
                var precios = $($(".fieldPreferencia").get(i)).find(".txtPrecioHora");

                // Este arreglo guarda las modalidades seleccionadas para luego insertarlas en el objeto e
                var modAux = [];

                for (var j = 0; j < modalidades.length; j++) {

                    // Sí está seleccionada dicha modalidad, que lea el textField de precioHora
                    if ($($(modalidades).get(j)).prop('checked')) {

                        var m = new Object();
                        m.id = $($(modalidades).get(j)).val();
                        m.precioHora = $($(precios).get(j)).val();
                        modAux.push(m);
                    }
                }
                e.modalidades = modAux;

                arreglo[i] = e;

            }

            $("#preferencias").val(JSON.stringify(arreglo));
            document.forms[0].submit;


        } else {

            alert("Debe llenar todos los campos y subir su foto");


        }



    }


});

$("#btnListo").unbind("click").on("click", function () {
    $("#modalMensaje").modal('hide');
    $("#txtModalMensaje").text("");

});

$(".btn-toolbar").on('click', '#btnCancelar', function (e) {

    $("#formBienvenida").submit();

});


// placeholders for the L.marker and L.circle representing user's current position and accuracy
var current_position, current_accuracy;

//cargarMapa();
function cargarMapa() {

    try {
        // Cargo mi mapa
        map = L.map('mapid');

    
    map.invalidateSize();

    L.tileLayer('https://api.mapbox.com/v4/{id}/{z}/{x}/{y}.{format}?access_token={accessToken}', {
        attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, <a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
        maxZoom: 18,
        id: 'mapbox.streets',
        format: 'jpg70',
        accessToken: 'sk.eyJ1IjoiZ3JpY2FyZG92IiwiYSI6ImNqbnphd3FicjAwNHAzcG85bmowaWFyMDUifQ.CxTAN8PiT2jZkldTwukmPg'
    }).addTo(map);

    map.locate({ setView: true, maxZoom: 16 });

    map.on('locationfound', onLocationFound);
    map.on('locationerror', onLocationError);
    } catch (e) { }
}

function onLocationFound(e) {

    $("#latitud").val(e.latlng.lat);
    $("#longitud").val(e.latlng.lng);

    // if position defined, then remove the existing position marker and accuracy circle from the map
    if (current_position) {
        map.removeLayer(current_position);
        map.removeLayer(current_accuracy);
    }

    //var radius = e.accuracy / 2;

    current_position = L.marker(e.latlng, { draggable:'true'}).addTo(map)
        .bindPopup("Se ha detectado tu ubicación en este punto, pero puedes mover este marcador para establecer tu ubicación exacta").openPopup();

    //current_accuracy = L.circle(e.latlng, radius).addTo(map);

    current_position.on('dragend', function (event) {
        var position = current_position.getLatLng();
        current_position.setLatLng(position, {
            draggable: 'true'
        }).bindPopup('Posición establecida en latitud: ' + position.lat + ', longitud: ' + position.lng).update();
        $("#latitud").val(position.lat);
        $("#longitud").val(position.lng);
    });

    
}

function onLocationError(e) {

    // Si no me quiere dar su ubicación, lo mando a la isla San Lorenzo JAJAJAJAJA
    $("#latitud").val('-12.088674392992298');
    $("#longitud").val('-77.21981048583986');

    // if position defined, then remove the existing position marker and accuracy circle from the map
    if (current_position) {
        map.removeLayer(current_position);
        map.removeLayer(current_accuracy);
    }

    //var radius = e.accuracy / 2;

    current_position = L.marker({ lat: '-12.088674392992298', lng: '-77.21981048583986' }, { draggable: 'true' }).addTo(map)
        .bindPopup("No se pudo establecer tu ubicación, pero puedes mover este marcador para establecerlo manualmente").openPopup();

    //current_accuracy = L.circle(e.latlng, radius).addTo(map);

    current_position.on('dragend', function (event) {
        var position = current_position.getLatLng();
        current_position.setLatLng(position, {
            draggable: 'true'
        }).bindPopup('Posición establecida en latitud: ' + position.lat + ', longitud: ' + position.lng).update();
        $("#latitud").val(position.lat);
        $("#longitud").val(position.lng);
    });


}    
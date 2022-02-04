import { map } from 'jquery';
import L from 'leaflet';

//Marcadores seleccionados
var aSeleccionados = [];

//Todos los marcadores
var aMarcadores = [];

//Variables para cambiar el color de los marcadores
var redIcon = new L.Icon({
    iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',
    shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowSize: [41, 41]
});
var blueIcon = new L.Icon({
    iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-blue.png',
    shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowSize: [41, 41]
});

//Si hay token o el token es valido se mostrara el mapa... si no se mostrara la parte del login para poder loguearse
comprobarToken();
function comprobarToken(){
    fetch("https://localhost:5001/api/InformacionTiempoes", {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem("token")}`,
        }
    }).then(response => {
        if (response.ok) {
            $("#inicioSesion").show();
            obteniendoDatos();
        }else{
            $("#login").show();
        }
    })
}

//Funcion para iniciar sesion, la cual mostrara los datos si el usuario y contraseña estan bien
document.getElementById("btn-login").addEventListener("click", inicioSesion);
function inicioSesion() {
    fetch("https://localhost:5001/Users/authenticate", {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            "username": $("#UsernameX").val(),
            "password": $("#PasswordX").val(),
        }),
    }).then(response => {
            if (response.ok) {
                return response.json()
            }
        })
        .then(response => {
            localStorage.setItem("token", response.token)
            $("#login").hide();
            $("#inicioSesion").show();
            obteniendoDatos();
        })
        .catch(err => {
            window.alert("Usuario o contraseña mal introducida");
            console.log("Usuario o contraseña mal introducida");
        });
}

//Obteniendo los datos de euskalmet teniendo un token
function obteniendoDatos() {
    fetch("https://localhost:5001/api/InformacionTiempoes", {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem("token")}`,
        }
    }).then(response => response.json())
        .then(aDatos => {
            //mapa
            const mapa = L.map('map').setView([43.29834714763016, -1.8620285690466898], 11);

            //"Comentarios" de la parte inferior derecha del mapa
            L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '<a href="http://185.60.40.210/2daw3/anderr/">Mi portfolio</a> &copy; ',
                maxZoom: 18
            }).addTo(mapa);
            
            //Funcion para crear los marcadores y el localStorage
            function crearMarcadores() {
                if (localStorage.IDs == null)
                    localStorage.IDs = JSON.stringify(aSeleccionados);
                for (let i = 0; i < aDatos.length; i++) {
                    let sMarker = L.marker([aDatos[i].gpxY, aDatos[i].gpxX], { myId: aDatos[i].id }).bindPopup(`${aDatos[i].nombre}`).addTo(mapa);
                    sMarker.on("click", añadirSeleccionado);
                    popUpOverOut(sMarker);
                    aMarcadores.push(sMarker);
                }
            };

            //Funcion para saber que marcador a clicado
            function añadirSeleccionado(e) {
                var sObtenerNombre = e.target.getPopup().getContent();
                for (let i = 0; i < aDatos.length; i++) {
                    if (aDatos[i].nombre == sObtenerNombre) {
                        let sId = aDatos[i].id;
                        //Se añadira a un array para saber si esta seleccionado 
                        if (aSeleccionados.indexOf(sId) == -1 && aSeleccionados.length < 4) {
                            aMarcadores[i].setIcon(redIcon);
                            var data = [sId, "true", "false", "false", "false"];
                            aSeleccionados.push(data);
                            localStorage.IDs = JSON.stringify(aSeleccionados);
                            crearSeleccionado(sId, aDatos);
                            borrarSeleccionada(aDatos);
                            activarDroppable();
                        }
                        break;
                    }
                }
            }

            //Funcion para crear los filtros
            function selectsFiltro() {
                let sSelecccionEstacion = `<select id="selEstacion" name="Estaciones">
                <option value="none">Estaciones</option>
                <option value="BUOY">Plataformas</option>
                <option value="METEOROLOGICAL">Meteorologico</option>
                <option value="GAUGING">De aforo</option>
                <option value="QUALITY">De calidad</option>
                </select>
                `;
                let sSelecccionProvincia = `<select id="selProvincia" name="Provincias">
                <option value="none">Provincias</option>
                <option value="Bizkaia">Bizkaia</option>
                <option value="Gipuzkoa">Gipuzkoa</option>
                <option value="Álava">Álava</option>
                <option value="Burgos">Burgos</option>
                <option value="Navarra">Navarra</option>
                </select>
                `;
                $("#filtro").append(sSelecccionEstacion);
                $("#filtro").append(sSelecccionProvincia);

                $("select").on("change", function () {
                    let sCambioEstaciones = document.getElementById("selEstacion").value;
                    let sCambioProvincias = document.getElementById("selProvincia").value;

                    //Eliminamos del mapa los markers para añadirlo de nuevo segun el filtro
                    aMarcadores.forEach(i => {
                        mapa.removeLayer(i);
                    });
                    aMarcadores = [];

                    //If else para saber la seleccion de filtro
                    if (sCambioEstaciones == "none" && sCambioProvincias == "none") {
                        crearMarcadores();
                    } else if (sCambioEstaciones == "none" && sCambioProvincias != "none") {
                        for (let i = 0; i < aDatos.length; i++) {
                            if (aDatos[i].provincia == sCambioProvincias) {
                                añadirMakerAMapa(i);
                            } else {
                                añadirMarkerArray(i);
                            }
                        }
                    } else if (sCambioEstaciones != "none" && sCambioProvincias == "none") {
                        for (let i = 0; i < aDatos.length; i++) {
                            if (aDatos[i].tipoEstacion == sCambioEstaciones) {
                                añadirMakerAMapa(i);
                            } else {
                                añadirMarkerArray(i);
                            }
                        }
                    } else {
                        for (let i = 0; i < aDatos.length; i++) {
                            if (aDatos[i].provincia == sCambioProvincias && aDatos[i].tipoEstacion == sCambioEstaciones) {
                                añadirMakerAMapa(i);
                            } else {
                                añadirMarkerArray(i);
                            }
                        }
                    }
                    colorearSeleccionados();
                });
            }

            //Dos Funciones para añadir al array de marcadores los markers y añadir al mapa los elegidos
            function añadirMakerAMapa(i) {
                let sMarker = L.marker([aDatos[i].gpxY, aDatos[i].gpxX], { myId: aDatos[i].id }).bindPopup(`${aDatos[i].nombre}`).addTo(mapa);
                sMarker.on("click", añadirSeleccionado);
                popUpOverOut(sMarker);
                aMarcadores.push(sMarker);
            }
            function añadirMarkerArray(i) {
                let sMarker = L.marker([aDatos[i].gpxY, aDatos[i].gpxX], { myId: aDatos[i].id }).bindPopup(`${aDatos[i].nombre}`);
                popUpOverOut(sMarker);
                aMarcadores.push(sMarker);
            }

            //Funcion para mostrar el popup y dejarlo de mostrar al estar encima de un marcador
            function popUpOverOut(sMarker) {
                sMarker.on("mouseover", function (e) {
                    this.openPopup();
                });

                sMarker.on("mouseout", function (e) {
                    this.closePopup();
                });
            }

            //Llamada de functions
            crearMarcadores();
            almacenadosLocalStorage(aDatos);
            selectsFiltro();
        })
};

//Se añade al html el seleccionado
function crearSeleccionado(sId, aDatos) {
    for (let i = 0; i < aDatos.length; i++) {
        if (aDatos[i].id == sId) {
            //Si hay datos se mostrara estos iconos si no, no aparecera nada
            let sTemp = "&deg;C";
            let sHume = "%";
            let sVient = "km/h";
            let sPreci = "mm=l/m²";
            if (aDatos[i].temperatura == "No hay datos") {
                sTemp = "";
            }
            if (aDatos[i].humedad == "No hay datos") {
                sHume = "";
            }
            if (aDatos[i].velocidadViento == "No hay datos") {
                sVient = "";
            }
            if (aDatos[i].precipitacionAcumulada == "No hay datos") {
                sPreci = "";
            }

            let sCrearDiv =
                `
            <div id="${sId}" class="opcionElegida ">
                <div id="elegida-info" class="d-flex flex-row">
                    <h3>${aDatos[i].nombre}</h3>
                    <button type="button" class="btn-close" aria-label="Close"></button>
                </div>
                <hr>
                <div class="informacion-cuadrado mostrar-info" id="divTemperature">
                    <p>Temperatura:</p>
                    <b><p>${aDatos[i].temperatura} ${sTemp}</p></b>
                </div>
                <div class="informacion-cuadrado" id="divHumidity">
                    <p>Humedad:</p>
                    <b><p>${aDatos[i].humedad}${sHume}</p></b>
                </div>
                <div class="informacion-cuadrado" id="divWind">
                    <p>Viento:</p>
                    <b><p>${aDatos[i].velocidadViento} ${sVient}</p></b>
                </div>
                <div class="informacion-cuadrado" id="divRaining">
                    <p>Precipitacion:</p>
                    <b><p>${aDatos[i].precipitacionAcumulada} ${sPreci}</p></b>
                </div>
            </div>
            `;
            document.getElementById("seleccionados").innerHTML += sCrearDiv;
        }
    }
}

//Obtenemos datos de los almacenados en el local storage
function almacenadosLocalStorage(aDatos) {
    if (localStorage.length != null) {
        let aAñadirArray = JSON.parse(localStorage.IDs);
        for (let i = 0; i < aAñadirArray.length; i++) {
            for (let j = 0; j < aDatos.length; j++) {
                if (aMarcadores[j].options.myId == aAñadirArray[i][0]) {
                    aMarcadores[j].setIcon(redIcon);
                }
            }
            let data = [aAñadirArray[i][0],aAñadirArray[i][1],aAñadirArray[i][2],aAñadirArray[i][3],aAñadirArray[i][4]]
            aSeleccionados.push(data);
            crearSeleccionado(aAñadirArray[i][0], aDatos);
            meterDatos(aAñadirArray[i][0]);  
        }
        borrarSeleccionada(aDatos);
        activarDroppable();
    }
}

//Borramos el seleccionado
function borrarSeleccionada(aDatos) {
    $(".btn-close").on("click", function () {
        //Obtenemos el id y posicion del id en seleccionados y borramos del array de seleccionados
        let sId = this.closest(".opcionElegida").id;
        for(let i = 0; i < aSeleccionados.length;i++){
            let sIdx = aSeleccionados[i][0].indexOf(sId);
            if (sIdx != -1) aSeleccionados.splice([i], 1);
        }
        //Actualizamos el localStorage
        for (let i = 0; i < aDatos.length; i++) {
            if (aDatos[i].id == sId) {
                localStorage.IDs = JSON.stringify(aSeleccionados);
                aMarcadores[i].setIcon(blueIcon);
                break;
            }
        }
        $(this).closest(".opcionElegida").remove();
    });
}

//Meter los datos en las tarjetas
function meterDatos(sId){
    for(let j = 0; j < aSeleccionados.length;j++){
        let sIdx = aSeleccionados[j][0].indexOf(sId);
        if (sIdx != -1){
            if(aSeleccionados[j][1] == "true"){
                $(`#${sId}`).find("#divTemperature").addClass("mostrar-info");
            }
            if(aSeleccionados[j][2] == "true"){
                $(`#${sId}`).find("#divHumidity").addClass("mostrar-info");
            }
            if(aSeleccionados[j][3] == "true"){
                $(`#${sId}`).find("#divWind").addClass("mostrar-info");
            }
            if(aSeleccionados[j][4] == "true"){
                $(`#${sId}`).find("#divRaining").addClass("mostrar-info");
            }
        }
    }
}

//Activamos el droppable
function activarDroppable() {
    $(".opcionElegida").droppable({
        drop: function (event, ui) {
            let papelera = ui.draggable.attr("id");
            let sIdTipoDeDato = ui.draggable.attr("id").substring(3);
            if (papelera == "Papelera") {
                $(this).find(`#divHumidity`).removeClass("mostrar-info");
                $(this).find(`#divWind`).removeClass("mostrar-info");
                $(this).find(`#divRaining`).removeClass("mostrar-info");
                for (let i = 0; i < aSeleccionados.length; i++) {
                    console.log("borrando")
                    let sIdx = aSeleccionados[i][0].indexOf(this.id);
                    if (sIdx != -1) {
                        aSeleccionados[i][2] = "false";
                        aSeleccionados[i][3] = "false";
                        aSeleccionados[i][4] = "false";
                    }
                }
            }else{
                $(this).find(`#div${sIdTipoDeDato}`).addClass("mostrar-info");
                for(let i = 0; i < aSeleccionados.length;i++){
                    let sIdx = aSeleccionados[i][0].indexOf(this.id);
                    if (sIdx != -1){
                        switch(sIdTipoDeDato){
                            case "Temperature":
                                aSeleccionados[i][1] = "true";
                                break;
                            case "Humidity":
                                aSeleccionados[i][2] = "true";
                                break;
                            case "Wind":
                                aSeleccionados[i][3] = "true";
                                break;
                            case "Raining":
                                aSeleccionados[i][4] = "true";
                                break;    
                        }
                    }
                }
            }
            localStorage.IDs = JSON.stringify(aSeleccionados);
        }
    });
}

//Funcion para colorear los marcadores seleccionados al filtrar
function colorearSeleccionados() {
    for (let i = 0; i < aMarcadores.length; i++) {
        for (let j = 0; j < aSeleccionados.length; j++) {
            if (aSeleccionados[j][0] == aMarcadores[i].options.myId) {
                aMarcadores[i].setIcon(redIcon);
            }
        }
    }
}

$(document).ready(function () {
    //Activamos el draggable en las imagenes de las opciones
    $("#Papelera").draggable({ helper: "clone" });
    $("#imgTemperature").draggable({ helper: "clone" });
    $("#imgHumidity").draggable({ helper: "clone" });
    $("#imgWind").draggable({ helper: "clone" });
    $("#imgRaining").draggable({ helper: "clone" });

    //Para minimizar el mapa
    $("#mini-map").on("click", function () {
        $("#map-info").slideToggle(1000);
    });

    //Al cerrar sesion se borrara el token y se recargara la pagina
    $("#CerrarSesion").on("click", function () {
        localStorage.setItem("token", "");
        location.reload();
    });
});

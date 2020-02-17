function toggleLayers() {
    let cover = document.getElementById("back-cover").style.visibility;

    document.getElementById("add-player-prompt").style.visibility =
        (cover == "visible") ? "hidden" : "visible";

    document.getElementById("back-cover").style.visibility =
        (cover == "visible") ? "hidden" : "visible";
}

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/lobbyHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(function () {
    console.log("feliratkozas");
    connection.invoke("Subscribe", document.getElementById("playerName").value).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {

    return console.error(err.toString());
});

connection.on("Invited", function (roomname) {
    (function(roomname) {
        VanillaToasts.create({
            title: 'Meghívás',
            text: 'Meghívtak a ' + roomname + 'szobába',
            type: 'success', // success, info, warning, error   / optional parameter
            timeout: 5000, // hide after 5000ms, // optional paremter
            callback: () => {
                document.getElementById("invitingroom").innerText = roomname;
                document.getElementById("roomName").value = roomname;
                toggleLayers();
            }
        });
    }) (roomname);
   
});
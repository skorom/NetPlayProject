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

connection.on("Invited", function (roomName) {
    document.getElementById("invitingroom").innerText = roomName;
    document.getElementById("roomName").value = roomName;
    toggleLayers();
});
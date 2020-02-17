// az emberek adatai
let records = [];

// a táblázat amihez az új elemeket hozzáfüzzük vagy ahonnan régieket letöröljük
let body = document.querySelector("#players").querySelector("tbody");

/*//kapcsolat a szerverrel
let connection = new signalR.HubConnectionBuilder()
    .withUrl("/lobbyHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();
document.addEventListener('DOMContentLoaded', () => { connection.start() });*/


 // a felugró ablakon a Mégse gomb kattintása
 document.querySelector("#cancel-player-addition")
    .addEventListener("click", e => {
      toggleLayers();
      // a felgró ablakban lévő mező törlése
        document.querySelector("#inviteName").value = "";
    });

  // a felugró ablakon a Meghívás gomb kattintása
 document.querySelector("#inviteButton").addEventListener("click", Invite);

  // a felugró ablakban enterrel való játékos meghívás
 document.querySelector("#inviteButton").addEventListener("keypress", e => {
     if (e.keyCode == 13) {     // 13 as az enter karakterkódja
        Invite();
    }
 });
    
// robot hozzáadása frontenden
document.querySelector("#add-robot").addEventListener('click', e => {
    addRobotToRoom();
});

// a plusz gombra kattintás lekezelése
document.querySelector("#add-player").addEventListener("click", e => {
    // felugró ablak megnyitása
    toggleLayers();
});

// Meghívás szerverre küldése
function Invite(){
    toggleLayers();
    if (document.querySelector("#inviteName").value == "") return;

    SendInvite();
    // a felgró ablakban lévő mező törlése
    document.querySelector("#inviteName").value = "";
}

// Játékos hozzáadása kliens oldali táblázathoz egy szerver object alapján
function addPlayer(player) {
  records.push(Record.makeRecord(player.name, player.score, player.role, player.state));
}

// egy játékos hozzáadása a kliens oldali táblázathoz ha még nem szerepel benne,
// ha igen akkor a meglévő elem módosítása
function editOrAddPlayer(player) {
    if (records.some((x)=> x.name == player.name)) {
        editPlayer(player.name, status = player.state);
    } else {
        addPlayer(player);
    }
}

// a felugró ablak elrejtése vagy megjelenítése
function toggleLayers() {
    let cover = document.querySelector("#back-cover").style.visibility;
    document.querySelector("#add-player-prompt").style.visibility =
    cover == "visible" ? "hidden" : "visible";
    document.querySelector("#back-cover").style.visibility =
    cover == "visible" ? "hidden" : "visible";
}

/*// játékos törlése kliens oldalról
// TODO: Szerveroldali törlés
function removePlayer(id) {
    let current = records.findIndex(e => e.id == id);
    if (current == -1) return;
    current = records[current];

    // kliensoldali törlés
    current.element.parentNode.removeChild(current.element);
    records.splice(id - 1, 1);
    recalculateIDs();


    if (current.status != "Elutasítva") {
    // TODO: a szerveroldalról is törölni kell a meghívást, vagy a szobából a játékost amenyiben már elfogadta a meghívást
        
    }
}*/

// egy játékos adatait változtatja
// név alapján keresi meg a játékost és az állapotát és a pontjait képes változtatni
function editPlayer(name, status, score) {
    let current = records.findIndex(e => e.name == name);
    if (current == -1) return;
    current = records[current];

    if (status != null) current.status = status;
    
    if (score != null) current.score = score;
    
    if (status || score) current.rebuild();
}

// kliensoldali id-k újbólili kiosztása
function recalculateIDs() {
  for (let index = 0; index < records.length; index++) {
    records[index].id = index + 1;
    records[index].rebuild();
  }
}

// az előzőleg csatlakozott emberek eltároláása
function getPrevInfo() {
    Array.from(body.querySelectorAll("tr")).slice(1).forEach((elem) => {
        records.push(Record.transformExistingModel(elem));
    });
}

getPrevInfo();
// az emberek adatai
let records = [];

// a táblázat amihez az új elemeket hozzáfüzzük vagy ahonnan régieket letöröljük
let body;

// egyszer fut le amint az oldal betöltődött
document.addEventListener("DOMContentLoaded", () => {
  // a felugró ablakon a Mégse gomb kattintása
  document
    .querySelector("#cancel-player-addition")
    .addEventListener("click", e => {
      toggleLayers();
      // a felgró ablakban lévő mező törlése
      document.querySelector("#add-player-name").value = "";
    });

  // a felugró ablakon a Meghívás gomb kattintása
  document.querySelector("#invite-player").addEventListener("click", e => {
    toggleLayers();
    if(document.querySelector("#add-player-name").value=="") return;
    invitePlayer(document.querySelector("#add-player-name").value);
    // a felgró ablakban lévő mező törlése
    document.querySelector("#add-player-name").value = "";

    //TODO: server connection , send message, wait for repyl
  });

  // a felugró ablakban enterrel való játékos meghívás
  document.querySelector("#add-player-name").addEventListener("keypress", e => {
    if (e.keyCode == 13) {
      // 13 as az enter karakterkódja
      toggleLayers();
      if(document.querySelector("#add-player-name").value=="") return;
      
      invitePlayer(document.querySelector("#add-player-name").value);
      // a felgró ablakban lévő mező törlése
      document.querySelector("#add-player-name").value = "";
    }
  });

  // a plusz gombra kattintás lekezelése
  document.querySelector("#add-player").addEventListener("click", e => {
    // felugró ablak megnyitása
    toggleLayers();
  });

  body = document.querySelector("#players").querySelector("tbody");

  // ez nem kerül majd be a végleges projectbe
  fillWithDummyData();
});

// játékosnak meghívót küldő függvény
function invitePlayer(name) {
  records.push(new Record(name, "", "Játékos", "Folyamatban"));
  // TODO: szerveren is meghívó küldése az adott játékosnak
}

// a felugró ablak elrejtése vagy megjelenítése
function toggleLayers() {
  let cover = document.querySelector("#back-cover").style.visibility;
  document.querySelector("#add-player-prompt").style.visibility =
    cover == "visible" ? "hidden" : "visible";
  document.querySelector("#back-cover").style.visibility =
    cover == "visible" ? "hidden" : "visible";
}

// a függvény akkkor hívódik meg amikor a meghívott fél válaszolt
function addPlayer(res) {
  let current =
    records.filter(elem => (elem.name = res.player.name))[0] || null;
  if (current == null) {
    // TODO: message a szervernek , a játékos már nincs meghívva a szobába
    return;
  }

  if (res.accepted) {
    editPlayer(current.name, "Kész", res.player.score);
  } else {
    editPlayer(current.name, "Elutasítva", res.player.score);
  }
}

// játékos törlése kliens oldalról és szerveroldalról is
function removePlayer(id) {
  // az id alapján megkeresi a játékost
  let current = records.filter(e => e.id == id)[0] || null;
  if (current == null) return;
  // kliensoldali törlés
  current.element.parentNode.removeChild(current.element);
  records.splice(id - 1, 1);
  recalculateIDs();

  if (current.status != "Elutasítva") {
    // TODO: a szerveroldalról is törölni kell a meghívást, vagy a szobából a játékost amenyiben már elfogadta a meghívást
  }
}

// egy játékos adatait változtatja
// név alapján keresi meg a játékost és az állapotát és a pontjait lehet változtatni
function editPlayer(name, status, score) {
  let current = records.filter(e => e.name == name)[0] || null;
  if (current == null) return;
  if (status != null) {
    current.status = status;
  }
  if (score != null) {
    current.score = score;
  }
  if (status || score) {
    current.rebuild();
  }
}

// kliensoldali id-k újbólili kiosztása
function recalculateIDs() {
  for (let index = 0; index < records.length; index++) {
    records[index].id = index + 1;
    records[index].rebuild();
  }
}

// teszt adatok feltöltése
function fillWithDummyData() {
  records.push(
    new Record("Én", parseInt(Math.random() * 3000), "Jákékos", "Kész")
  );
  records.push(
    new Record("Jani", parseInt(Math.random() * 3000), "Néző", "Folyamatban")
  );
  records.push(
    new Record("Bence", parseInt(Math.random() * 3000), "Játékos", "Kész")
  );
  records.push(
    new Record("Feri", parseInt(Math.random() * 3000), "Játékos", "Elutasítva")
  );
  records.push(
    new Record("Bálint", parseInt(Math.random() * 3000), "Néző", "Folyamatban")
  );
  records.push(
    new Record("TMSKSS", parseInt(Math.random() * 3000), "Néző", "Kész")
  );
  records.push(
    new Record("H.balint", parseInt(Math.random() * 3000), "Néző", "Elutasítva")
  );
}

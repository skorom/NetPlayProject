// egy játékos adatai
class Record {
    static makeRecord(name, score, role, status) {
        let r = new Record();
        r.name = name;
        r.score = score;
        r.status = status;
        r.role = role;

        // id automatikus kiosztása inicializáláskor
        r.id = body.querySelectorAll("tr").length;

        // A kliensoldali táblázatban megjelenő elem
        r.element = document.createElement("tr");
        r.rebuild();
        // az elem hozzáadása a táblázathoz
        body.append(r.element);
        return r;
    }

    // a szerver által betöltött adatokat tárolja el
    static transformExistingModel(element) {
        let data = Array.from(element.querySelectorAll("td")).map(x => x.innerText);
        let r = new Record();
        r.name = data[1];
        r.score = data[2];
        r.role = data[3];
        r.status = data[4];

        // id automatikus kiosztása inicializáláskor
        r.id = parseInt(data[0]);

        // A kliensoldali táblázatban megjelenő elem
        r.element = element
        r.rebuild();

        return r;
    }

    // az elem tartalmának létrehozása
    rebuild(){
        this.element.innerHTML = `
        <th>${this.id}</th>
        <td>${this.name}</td>
        <td>${this.score}</td>
        <td>${this.role}</td>
        <td>${this.status}</td>
        <td id="${this.id}" class="mx-0 px-0 clearfix">
            <button class="d-flex justify-content-center align-items-center btn btn-danger h-auto" onClick="removePlayer(${this.id})" >X</button>
        </td>
        `;
    } 

}
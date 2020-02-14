// egy játékos adatai
class Record {
  constructor(name, score, role, status) {
    this.name = name;
    this.score = score;
    this.status = status;
    this.role = role;
    // id automatikus kiosztása inicializáláskor
    this.id =
      (parseInt(
        Array.from(body.querySelectorAll("tr"))
          .pop()
          .querySelector("th").textContent
      )) + 1 || 1;

    // A kliensoldali táblázatban megjelenő elem
    this.element = document.createElement("tr");
    this.rebuild();
    // az elem hozzáadása a táblázathoz
    body.append(this.element);

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
            <button class="float-right btn btn-danger mr-2" onClick="removePlayer(${this.id})" ><i class="fa fa-times"></i></button>
        </td>
        `;
    } 

}
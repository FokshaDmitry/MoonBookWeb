let library;
document.addEventListener("DOMContentLoaded", () => {
	//Element <library><library/>
	library = document.querySelector("library");
	if (!library) throw "Forum  script: APP not found";
	library.innerHTML = `<img style="width: 150px; margin-left: 30%; position: absolute; margin-top: 15%;" src ="../icons/oie_L36bLHLNsDH2.gif"/>`
	//Load All books
	loadElement(library)
});
let Search = document.getElementById("SearchBook");
let Genry = document.getElementById("Genry");
let Rating = document.getElementById("Rating");
let Date = document.getElementById("DateBook");
let Alphabet = document.getElementById("AlpBook");
Rating.addEventListener("change", () => {
	loadElement(library);
})
Genry.addEventListener("change", () => {
	loadElement(library);
})
Date.addEventListener("change", () => {
	loadElement(library);
})
Alphabet.addEventListener("change", () => {
	loadElement(library);
})
Search.addEventListener("click", () => {
	let text = document.getElementById("SearchAllBook").value;
	if (text !== "") {
		fetch(`/api/library/${text}`, {
			method: "GET",
			body: null
		}).then(r => r.json())
			.then(j => {
				if (j.status === "Error") {
					alert(j.message)
				}
				else {
					showElement(library, j.message);
				}
			});
	}
	else {
		location.reload();
    }
})

function loadElement(elem) {
	const formData = new FormData();
	//Filter
	formData.append("Genry", (document.getElementById("Genry").value == "Choose Genry" ? "" : document.getElementById("Genry").value))
	formData.append("Date", document.getElementById("DateBook").checked)
	formData.append("Alphabet", document.getElementById("AlpBook").checked)
	formData.append("Rating", document.getElementById("Rating").checked)
	fetch(`/api/library`, {
			method: "POST",
			body: formData
		}).then(r => r.json())
		.then(j => {
			if (j.status === "Ok") {
				showElement(elem, j.message);
			}
			else {
				throw "showTopics: Backend data invalid";
			}
		});
}
function showElement(elem, j) {
	fetch("/tmpl/book.html")
		.then(r => r.text())
		.then(trTemplate => {
			let appHtml = "";
			for (let book of j) {
				var tmp = trTemplate
				tmp = tmp.replaceAll("{{id}}", book.book.id)
					.replaceAll("{{Title}}", book.book.title)
					.replaceAll("{{Author}}", book.book.author)
					.replaceAll("{{CoverName}}", (book.book.coverName == null || book.book.coverName == "" ? "import_contacts_FILL0_wght400_GRAD0_opsz48.png" : book.book.coverName))
					.replaceAll("{{Star}}", (book.rating.length !== 0 ? ((book.rating.map(b => b.grade).reduce((x, y) => x + y, 0)) / book.rating.length).toFixed(1) : 0));
				appHtml += tmp;
			}
			elem.innerHTML = appHtml;
			bookLoaded();
		});
}
async function bookLoaded() {
	for (let book of document.querySelectorAll(".Books")) {
		book.onclick = Read;
	}
}
function Read(e) {
	let idBook = e.currentTarget.getAttribute("id")
	window.location.href = `/Books/BookPage?${idBook}`;
}
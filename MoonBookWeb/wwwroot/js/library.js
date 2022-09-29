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
let Date = document.getElementById("DateBook");
let Alphabet = document.getElementById("AlpBook");
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
	fetch(`/api/library/books`,{
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
				tmp = tmp.replaceAll("{{id}}", book.id)
					.replaceAll("{{Title}}", book.title)
					.replaceAll("{{Author}}", book.author)
					.replaceAll("{{CoverName}}", (book.coverName == null || book.coverName == "" ? "import_contacts_FILL0_wght400_GRAD0_opsz48.png" : book.coverName))
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
	window.location.href = `/Books/UserLibrary?${idBook}`;
}
document.addEventListener("DOMContentLoaded", () => {
	let library = document.querySelector("library");
	if (!library) throw "Forum  script: APP not found";
	library = document.querySelector("library");
	loadElement(library)
});

function loadElement(elem) {
	fetch(`/api/library`,
		{
			method: "GET",
			body: null
		})
		.then(r => r.json())
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
		});
}
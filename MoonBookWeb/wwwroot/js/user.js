document.addEventListener("DOMContentLoaded", () => {
	const online = document.querySelector("online");
	if (!online) throw "Forum  script: APP not found";
	loadElement(online);
});
function loadElement(elem) {
	fetch(`/api/user`,
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
	fetch("/tmpl/onlineFrands.html")
		.then(r => r.text())
		.then(trTemplate => {
			let appHtml = "";
			for (let freand of j) {
				var tmp = trTemplate
				tmp = tmp.replaceAll("{{id}}", freand.id)
					.replaceAll("{{PhotoName}}", (freand.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : freand.photoName))
				appHtml += tmp;
			}
			elem.innerHTML = appHtml;
			this.freandLoaded();
		});
}
async function freandLoaded() {
	for (let freand of document.querySelectorAll(".idOnlineFreand")) {
		freand.onclick = View;
	}
}
function View(e) {
	let idFreand = e.currentTarget.getAttribute("id")
	window.location.href = `/User/FreandPage?${idFreand}`;
}
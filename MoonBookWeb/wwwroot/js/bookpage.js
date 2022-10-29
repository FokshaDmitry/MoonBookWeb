document.addEventListener("DOMContentLoaded", () => {
	link = new URL(window.location.href).search.replace('?', "");
	//if link not empty, show choose user
	if (link != '') {
		bookpageLoaded(link);
	}
});
let link;
let read = document.getElementById("Read");
let follow = document.getElementById("Follow");
let send = document.getElementById("Send");
function bookpageLoaded(link) {
	fetch(`/api/BookPage/${link}`, {
		method: "GET",
		body: null
	}).then(r => r.json()).then(j => {
		if (j.status === "Error") {
			alert(j.message)
		} else {
			document.getElementById("Title").innerText = j.message.title
			document.getElementById("Author").textContent += j.message.author
			document.getElementById("Genry").textContent += j.message.genry
			document.getElementById("Date").textContent += j.message.date
			document.getElementById("CoverBook").setAttribute("src", `../img_post/${j.message.coverName}`)
			document.getElementById("Follow").innerHTML = j.follow ? `<img src="../icons/bookmark_remove_FILL0_wght400_GRAD0_opsz48.png"> Remove Library` : `<img src="../icons/bookmark_add_FILL0_wght400_GRAD0_opsz48.png"> Add Library`
        }
    })
}
read.addEventListener("click", () => {
	window.location.href = `/Books/UserLibrary?${link}`;
})
follow.addEventListener("click", () => {
	fetch(`/api/book/${link}`,
		{
			method: "POST",
			body: null
		})
		.then(r => r.json())
		.then(j => {
			if (j.status === "Error") {
				throw `addBook: ${j.status}`;
			}
			else {
				location.reload();
			}
		});
})
send.addEventListener("click", () => {
	let Text = document.getElementById("TextComment");
	let answer;
	try {
		answer = commentAnswer.querySelector("a").getAttribute("href").replace("../User/FreandPage?", "")

	} catch (e) {
		answer = "";
	}
	if (Text.value != "") {
		const formData = new FormData();
		formData.append("Text", Text.value);
		formData.append("id", link);
		formData.append("Answer", answer)
		fetch("/api/bookpage", {
			method: "POST",
			body: formData
		}).then(r => r.json()).then(j => {
			if (j.status == "Error") {
				alert(j.message);
            }
		})
    }
})
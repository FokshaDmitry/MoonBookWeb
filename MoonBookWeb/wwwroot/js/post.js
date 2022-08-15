var Title = document.getElementById("Title");
var Text = document.getElementById("Text");
var Post = document.getElementById("Post");
var ImagePost = document.getElementById("ImagePost");
var valTitle = "";

document.addEventListener("DOMContentLoaded", () => {
	const post = document.querySelector("post");  
	if (!post) throw "Forum  script: APP not found";
	loadTopics(post);
});

function loadTopics(elem) {
	fetch("/api/post",  
		{
			method: "GET",
			headers: {
				"User-Id": "",
				"Culture": ""
			},
			body: null
		})
		.then(r => r.json())
		.then(j => {
			if (j instanceof Array) {
				showTopics(elem, j);
			}
			else {
				throw "showTopics: Backend data invalid";
			}
		});
}
function showTopics(elem, j) {
	fetch("/tmpl/post.html")
		.then(r => r.text())
		.then(trTemplate => {
			var appHtml = "";
			for (let post of j) {
				var tmp = trTemplate
				tmp = tmp
						.replace("{{Name}}", post.user.name)
						.replace("{{Surname}}", post.user.Surname)
						.replace("{{Date}}", post.post.date)
						.replace("{{Like}}", post.post.like)
						.replace("{{Dislike}}", post.post.dislike)
						.replace("{{Id}}", post.post.id);
				if (post.user.photoName != null) {
					tmp = tmp.replace("{{PhotoUser}}", post.user.photoName)
				} else {
					tmp = tmp.replace("{{PhotoUser}}", "android_contacts_FILL0_wght400_GRAD0_opsz48.png");
				}
				if (post.post.image != null) {
					tmp = tmp.replace("{{PostImg}}", `<img id="PostImg" src="/img_post/${post.post.image}" />`)
				} else {
					tmp = tmp.replace("{{PostImg}}", "");
				}
				if (post.post.text != null) {
					tmp = tmp.replace("{{Text}}", post.post.text)
				} else {
					tmp = tmp.replace("{{Text}}", "");
				}
				if (post.post.title != null) {
					tmp = tmp.replace("{{Title}}", post.post.title)
				} else {
					tmp = tmp.replace("{{Title}}", "");
				}
				appHtml += tmp;
			}
			elem.innerHTML = appHtml;
			postLoaded();
		});
}
async function postLoaded() {
	for (let post of document.querySelectorAll(".Post")) {
		post.onclick = Reactions;
	}
}
function Reactions(e) {
	let sad = e.currentTarget.childNodes[7].childNodes[1].firstElementChild;
	let smile = e.currentTarget.childNodes[7].childNodes[3].firstElementChild;
	let idPost = e.currentTarget.getAttribute("id")
	let dislike = e.currentTarget.childNodes[7].childNodes[1].childNodes[3]
	let like = e.currentTarget.childNodes[7].childNodes[3].childNodes[3]
	if (e.target === sad) {
		const formData = new FormData();
		formData.append("Reaction", 2);
		formData.append("IdPost", idPost);
		fetch("/api/post", {
			method: "PUT",
			body: formData
		}).then(r => r.json()).then(j => {
			if (j.status == "Error") {
				alert(j.message)
			} else {
				dislike.textContent = j.reactDislike;
			}	like.textContent = j.reactLike;
		})
	}
	if (e.target === smile) {
		const formData = new FormData();
		formData.append("Reaction", 1);
		formData.append("IdPost", idPost);
		fetch("/api/post", {
			method: "PUT",
			body: formData
		}).then(r => r.json()).then(j => {
			if (j.status == "Error") {
				alert(j.message)
			} else {
				dislike.textContent = j.reactDislike;
			} like.textContent = j.reactLike;
		})
	}
}
Title.addEventListener("click", () => {
	if (Text.selectionStart != Text.selectionEnd) {
		var start = Text.selectionStart;
		var end = Text.selectionEnd;
		valTitle = Text.value.substring(start, end);
		Text.value = Text.value.substr(0, start) + ">" + Text.value.substr(start, end) + "<" + Text.value.substr(end);
		Text.setSelectionRange(end, end);
	}
})
Post.addEventListener("click", () => {
	const formData = new FormData();
	formData.append("TitlePost", valTitle);
	formData.append("TextPost", Text.value);
	formData.append("ImagePost", ImagePost.files[0]);
	fetch("/API/Post", {
		method: "POST",
		body: formData
	}).then(r => r.json()).then(j => {
		if (j.status == "Error") {
			alert(j.message)
		} else {
			location.reload();
		}
	})
});

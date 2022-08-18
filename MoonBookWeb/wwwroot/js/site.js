
class Posts {
	constructor(elem, API) {
		this.elem = elem
		this.API = API;
	}
	loadElement() {
		fetch(this.API,
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
				if (j.status === "Ok") {
					this.showPosts(this.elem, j.message);
				}
				else {
					throw "showTopics: Backend data invalid";
				}
			});
	}
	loadElements(url) {
		fetch(`${this.API}/${url}`,
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
				if (j.status === "Ok") {
					this.showPosts(this.elem, j.message);
				}
				else {
					throw "showTopics: Backend data invalid";
				}
			});
	}
	showPosts(elem, j) {
		fetch("/tmpl/post.html")
			.then(r => r.text())
			.then(trTemplate => {
				var appHtml = "";
				for (let post of j) {
					var tmp = trTemplate;
					tmp = tmp.replace("{{Name}}", post.user.name)
							 .replace("{{Surname}}", post.user.surname)
							 .replace("{{Date}}", post.post.date)
							 .replace("{{Like}}", post.post.like)
							 .replace("{{Dislike}}", post.post.dislike)
							 .replace("{{Id}}", post.post.id)
							 .replace("{{PhotoUser}}", (post.user.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : post.user.photoName))
							 .replace("{{PostImg}}", (post.post.image == null ? "" : `<img id="PostImg" src="/img_post/${post.post.image}" />`))
							 .replace("{{Text}}", (post.post.text == null ? "" : post.post.text))
							 .replace("{{Title}}", (post.post.title == null ? "" : post.post.title))
					appHtml += tmp;
				}
				elem.innerHTML = appHtml;
				this.postLoaded();
			});
	}
	async postLoaded() {
		for (let post of document.querySelectorAll(".Post")) {
			post.onclick = this.Reactions;
		}
	}
	Reactions(e) {
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
					like.textContent = j.reactLike;
				}
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
					like.textContent = j.reactLike;
				}
			})
		}
	}
}
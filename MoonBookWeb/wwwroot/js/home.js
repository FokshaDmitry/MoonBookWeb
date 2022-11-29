document.addEventListener("DOMContentLoaded", () => {
	//Element <post><post/>
	let post = document.querySelector("post");
	if (!post) throw "Forum  script: APP not found";
	post.innerHTML = `<img style="width: 150px; position: absolute; margin-top: 15%; margin-left: -5%;" src ="../icons/oie_L36bLHLNsDH2.gif"/>`
	let posts = new Posts("/api/post/home");
	//load All Post
	posts.loadElement(post);
});
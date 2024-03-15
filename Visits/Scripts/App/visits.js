// Namespace
const visits = {};

visits.baseURL = (function () {
	const baseURL = (document.body.hasAttribute("data-baseurl") ? document.body.getAttribute('data-baseurl') : '/');

	var get = function () {
		return baseURL;
	};

	return {
		get: get,
	};
})();

visits.helpers = {};
visits.helpers.ucfirst = (text) => {
	return text.charAt(0).toUpperCase() + text.slice(1).toLocaleLowerCase();
};

visits.components = (function () {
	const components = {};

	var waitFor = function (componentName) {
		components[componentName] = {};
		components[componentName].isReady = false;
	};

	var isReady = function (componentName) {
		if (components[componentName] != null) {
			components[componentName].isReady = true;
			applyLoadCompletion();
		}
	};

	var applyLoadCompletion = function () {
		let isCompleted = true;
		for (var key in components) {
			if (!components[key].isReady) {
				isCompleted = false;
				break;
			}
		}
		if (isCompleted) {
			document.getElementById('body').classList.remove('displaynone');
			setTimeout(function () { document.body.children[0].classList.add('fade-out'); }, 500);
			setTimeout(function () { document.body.children[0].classList.add('displaynone');}, 1000);
		}
	};

	return {
		waitFor: waitFor,
		isReady: isReady,
	};
})();



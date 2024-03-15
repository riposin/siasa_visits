// Namespace
const visits = {};

visits.baseURL = (function () {
	const namespace = 'visits.baseURL';
	let base = '/';

	var baseURL = (function () {
		if (document.body.hasAttribute("data-baseurl")) {
			base = document.body.getAttribute('data-baseurl');
		} else {
			console.error(namespace + ': baseURL attr not found, used "/" instead.');
		}
		return base;
	})();

	var get = function () {
		return base;
	};

	return {
		get: get,
	};
})();
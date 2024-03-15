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
			setTimeout(function () { document.body.children[0].classList.add('displaynone'); }, 1000);
		}
	};

	return {
		waitFor: waitFor,
		isReady: isReady,
	};
})();
Visits.Language = (function(){
	const currentLangLSKey = 'currentLang';
	const translationVersionLSKey = 'translationVersion';
	const langSelectorID = 'languageSelector';
	const prefixTranslationsLS = 'LBLS_';
	const lblPageTitleSuffix = 'LBL_PTITLE_SUFFIX';
	let labels = {};
	let liCurrentLang;
	let aLangs;
	let currentLangDTFormat = '';

	var init = function () {
		initLS();
		initSelector();
		getLabelsAndApply();
	};

	var initLS = function () {
		if (localStorage.getItem(currentLangLSKey) === null) {
			localStorage.setItem(currentLangLSKey, document.getElementById(langSelectorID).getAttribute('data-default'));
		}

		if (localStorage.getItem(translationVersionLSKey) === null || localStorage.getItem(translationVersionLSKey) != document.getElementById(langSelectorID).getAttribute('data-version')) {
			localStorage.setItem(translationVersionLSKey, document.getElementById(langSelectorID).getAttribute('data-version'));
			Object.keys(localStorage).forEach(function (key) {
				if (key.startsWith(prefixTranslationsLS)) {
					localStorage.removeItem(key);
				}
			});
		}
	};

	var initSelector = function () {
		liCurrentLang = document.getElementById(langSelectorID).querySelectorAll('li[data-lang="' + localStorage.getItem(currentLangLSKey) + '"]')[0];
		liCurrentLang.classList.add("active");

		aLangs = document.getElementById(langSelectorID).querySelectorAll('ul li a');
		aLangs[0].children.item(0).innerHTML = liCurrentLang.getAttribute('data-display');
		currentLangDTFormat = liCurrentLang.getAttribute('data-format');

		for (let i = 1; i < aLangs.length; i++) {
			aLangs[i].addEventListener("click", onLangChangeClick);
		}
	};

	var getLabelsAndApply = function () {
		let currLangTransLSKey = prefixTranslationsLS + localStorage.getItem(currentLangLSKey);
		if (localStorage.getItem(currLangTransLSKey) === null) {
			fetch(Visits.baseURL + 'Language/Labels/' + localStorage.getItem(currentLangLSKey), {
				method: "GET",
				headers: { 'Accept': 'application/json' }
			})
			.then(function (response) {
				return response.json();
			})
			.then(function (json) {
				localStorage.setItem(prefixTranslationsLS + localStorage.getItem(currentLangLSKey), JSON.stringify(json[0]));
				applyLabels();
			})
			.catch(function (error) {
				alert('AJAX: Error when getting labels');
				location.reload();
			});
		} else {
			applyLabels();
		}
	};

	var applyLabels = function () {
		let elemsToTranslate = document.querySelectorAll('[data-translate]');
		let currLangTransLSKey = prefixTranslationsLS + localStorage.getItem(currentLangLSKey);
		let currLabel = '';
		let currTrans = '';


		if (labels[currLangTransLSKey] == null) {
			labels[currLangTransLSKey] = JSON.parse(localStorage.getItem(currLangTransLSKey));
		}

		for (let i = 0; i < elemsToTranslate.length; i++) {
			currLabel = elemsToTranslate[i].getAttribute('data-label');

			if (labels[currLangTransLSKey][currLabel] == null) {
				currTrans = currLabel;
			} else {
				currTrans = labels[currLangTransLSKey][currLabel];
			}

			if (elemsToTranslate[i].hasAttribute('data-translate-ucfirst')) {
				currTrans = currTrans.charAt(0).toUpperCase() + currTrans.slice(1).toLocaleLowerCase();
			}
			if (elemsToTranslate[i].hasAttribute('data-translate-inner')) {
				elemsToTranslate[i].innerHTML = currTrans;
			}
			if (elemsToTranslate[i].hasAttribute('data-translate-value')) {
				elemsToTranslate[i].setAttribute('value', currTrans);
			}
			if (elemsToTranslate[i].hasAttribute('data-translate-title')) {
				elemsToTranslate[i].setAttribute('title', currTrans);
			}
		}

		if (labels[currLangTransLSKey][lblPageTitleSuffix] == null) {
			document.title = document.title + lblPageTitleSuffix;
		} else {
			document.title = document.title + labels[currLangTransLSKey][lblPageTitleSuffix];
		}
	};

	var onLangChangeClick = function (event) {
		if (!this.parentNode.classList.contains('active')) {
			liCurrentLang.classList.remove('active');
			this.parentNode.classList.add('active');
			liCurrentLang = this.parentNode;
			aLangs[0].children.item(0).innerHTML = liCurrentLang.getAttribute('data-display');
			currentLangDTFormat = liCurrentLang.getAttribute('data-format');
			localStorage.setItem(currentLangLSKey, this.parentNode.getAttribute('data-lang'));
			getLabelsAndApply();
			document.getElementById(langSelectorID).dispatchEvent(new CustomEvent("languageChange", { detail: { lang: this.parentNode.getAttribute('data-lang'), format: currentLangDTFormat }}));
		}
	};

	var getCurrentFormat = function () {
		return currentLangDTFormat;
	};

	var getTranslation = function (label) {
		let currLangTransLSKey = prefixTranslationsLS + localStorage.getItem(currentLangLSKey);
		let translation = label;

		if (labels[currLangTransLSKey][label] != null) {
			translation = labels[currLangTransLSKey][label];
		}

		return translation;
	};

	return {
		init: init,
		selector: document.getElementById(langSelectorID),
		getCurrentFormat: getCurrentFormat,
		getTranslation: getTranslation,
	};
})();

Visits.Language.init();
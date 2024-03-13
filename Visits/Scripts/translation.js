const currentLangLSKey = 'currentLang';
const translationVersionLSKey = 'translationVersion';
const langSelectorID = 'languageSelector';
const prefixTranslationsLS = 'LBLS_';
let labels = {};
let liCurrentLang;
let aLangs;

if (localStorage.getItem(currentLangLSKey) === null) {
	localStorage.setItem(currentLangLSKey, document.getElementById(langSelectorID).getAttribute('data-default'));
}

if (localStorage.getItem(translationVersionLSKey) === null || localStorage.getItem(translationVersionLSKey) != document.getElementById(langSelectorID).getAttribute('data-version')) {
	initializeLabelsVersion();
}

liCurrentLang = document.getElementById(langSelectorID).querySelectorAll('li[data-lang="' + localStorage.getItem(currentLangLSKey) + '"]')[0];
liCurrentLang.classList.add("active");

aLangs = document.getElementById(langSelectorID).querySelectorAll('ul li a');
aLangs[0].innerHTML = liCurrentLang.getAttribute('data-display');

for (let i = 1; i < aLangs.length; i++) {
	aLangs[i].addEventListener("click", onLangChangeClick);
}

getLabelsAndApply();


/* ------ Methods section ------ */

function initializeLabelsVersion() {
	localStorage.setItem(translationVersionLSKey, document.getElementById(langSelectorID).getAttribute('data-version'));
	Object.keys(localStorage).forEach(function (key) {
		if (key.startsWith(prefixTranslationsLS)) {
			localStorage.removeItem(key);
		}
	});
}

function getLabelsAndApply() {
	let currLangTransLSKey = prefixTranslationsLS + localStorage.getItem(currentLangLSKey);
	if (localStorage.getItem(currLangTransLSKey) === null) {
		fetch(visits.baseURL + 'Language/Labels/' + localStorage.getItem(currentLangLSKey), {
			method: "GET",
			headers: {'Accept': 'application/json'}
		})
		.then(function (response) {
			return response.json();
		})
		.then(function (json) {
			localStorage.setItem(prefixTranslationsLS + localStorage.getItem(currentLangLSKey), JSON.stringify(json));
			applyLabels();
		})
		.catch(function (error) {
			alert('AJAX: Error when getting labels');
			location.reload();
		});
	} else {
		applyLabels();
	}
}

function applyLabels() {
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
}

function onLangChangeClick(event) {
	if (!this.parentNode.classList.contains('active')) {
		liCurrentLang.classList.remove('active');
		this.parentNode.classList.add('active');
		liCurrentLang = this.parentNode;
		aLangs[0].innerHTML = liCurrentLang.getAttribute('data-display');
		localStorage.setItem(currentLangLSKey, this.parentNode.getAttribute('data-lang'));
		getLabelsAndApply();
	}
}
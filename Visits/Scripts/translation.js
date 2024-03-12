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
		// Get LBLs from BE but only from the currentLangLSKey and save them to LS, the else is needed because this will be async, will be an AJAX call
		// Save the LS key using currLangTransLSKey
		localStorage.setItem(prefixTranslationsLS + 'es-MX', '[{"LBL_VISITS":"visitas","LBL_SCR_TITLE_REQ":"solicitud de pre-registro de visitantes"}]');
		localStorage.setItem(prefixTranslationsLS + 'en-US', '[{"LBL_VISITS":"visits","LBL_SCR_TITLE_REQ":"visitor pre-registration request"}]');

		applyLabels();
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
		labels[currLangTransLSKey] = JSON.parse(localStorage.getItem(currLangTransLSKey))[0];
	}

	for (let i = 0; i < elemsToTranslate.length; i++) {
		currLabel = elemsToTranslate[i].getAttribute('data-label');

		if (labels[currLangTransLSKey][currLabel] == null) {
			elemsToTranslate[i].innerHTML = currLabel;
		} else {
			elemsToTranslate[i].innerHTML = labels[currLangTransLSKey][currLabel];
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
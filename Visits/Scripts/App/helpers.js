visits.helpers = {};

visits.helpers.ucfirst = (text) => {
	return text.charAt(0).toUpperCase() + text.slice(1).toLocaleLowerCase();
};
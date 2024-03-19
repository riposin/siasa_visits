visits.helpers = {};

visits.helpers.ucfirst = (text, lowerize = true) => {
	return text.charAt(0).toUpperCase() + (lowerize ? text.slice(1).toLowerCase() : text.slice(1));
};
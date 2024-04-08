const toggleFiltersButton = document.getElementById("toggle-filters");
const filtersSection = document.getElementById("filters");

toggleFiltersButton.addEventListener('click', () => {
    if (filtersSection.classList.contains("open")) {
        filtersSection.classList.remove("open");
    } else {
        filtersSection.classList.add("open");
    }
});

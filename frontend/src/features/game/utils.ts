export const matchesFilters = (
  a: { isUnlocked: boolean; isProtected: boolean; isHidden: boolean },
  filters: Set<string>,
) => {
  if (filters.has('locked') && !a.isUnlocked) {
    return true;
  }
  if (filters.has('unlocked') && a.isUnlocked) {
    return true;
  }
  if (filters.has('protected') && a.isProtected) {
    return true;
  }
  if (filters.has('hidden') && a.isHidden) {
    return true;
  }

  return false;
};

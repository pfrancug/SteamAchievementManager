export interface StatEditingContextValue {
  activeStatId: string | null;
  onSave: (id: string, type: 'int' | 'float' | 'rate', value: string) => void;
  optimisticValues: Map<string, number>;
  savingIds: Set<string>;
  setActiveStatId: (id: string | null) => void;
}

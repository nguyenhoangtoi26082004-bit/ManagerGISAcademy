const React = __vite__cjsImport0_react; const useState = __vite__cjsImport0_react["useState"]; const useMemo = __vite__cjsImport0_react["useMemo"];const _jsxDEV = __vite__cjsImport17_react_jsxDevRuntime["jsxDEV"];/**
* @license
* SPDX-License-Identifier: Apache-2.0
*/
import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { INITIAL_MILITARY_UNITS } from "/src/data/militaryUnits.ts";
import { auditMilitaryData } from "/src/utils/gisCalculations.ts";
import { exportFullProjectZip } from "/src/utils/projectZipExporter.ts";
import { Header } from "/src/components/Header.tsx";
import { Sidebar } from "/src/components/Sidebar.tsx";
import { StatusBar } from "/src/components/StatusBar.tsx";
import { GisMap } from "/src/components/GisMap.tsx";
import { UnitDetailDrawer } from "/src/components/UnitDetailDrawer.tsx";
import { DashboardView } from "/src/components/DashboardView.tsx";
import { UnitSearchView } from "/src/components/UnitSearchView.tsx";
import { DataManagementView } from "/src/components/DataManagementView.tsx";
import { SpatialAnalysisView } from "/src/components/SpatialAnalysisView.tsx";
import { StatisticsView } from "/src/components/StatisticsView.tsx";
import { DataCheckerView } from "/src/components/DataCheckerView.tsx";
import { UnitModal } from "/src/components/UnitModal.tsx";
import { PostgisModal } from "/src/components/PostgisModal.tsx";
var _jsxFileName = "/app/applet/src/App.tsx";
import __vite__cjsImport17_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export default function App() {
	const [units, setUnits] = useState(() => {
		const saved = localStorage.getItem("hanoi_military_units_data");
		if (saved) {
			try {
				const parsed = JSON.parse(saved);
				if (Array.isArray(parsed) && parsed.length > 0) return parsed;
			} catch (e) {}
		}
		return INITIAL_MILITARY_UNITS;
	});
	// Current active tab (default to 'map' to match user's image.png)
	const [activeTab, setActiveTab] = useState("map");
	// Currently inspected military unit
	const [selectedUnit, setSelectedUnit] = useState(null);
	// Live cursor coordinates from Leaflet map
	const [cursorCoord, setCursorCoord] = useState(null);
	// Buffer analysis state
	const [bufferConfig, setBufferConfig] = useState({
		active: false,
		centerLat: 0,
		centerLng: 0,
		radiusKm: 10
	});
	// Filters
	const [filterBranch, setFilterBranch] = useState(undefined);
	// Modals
	const [isAddEditModalOpen, setIsAddEditModalOpen] = useState(false);
	const [editingUnit, setEditingUnit] = useState(null);
	const [isPostgisModalOpen, setIsPostgisModalOpen] = useState(false);
	// Save to localStorage whenever units update
	const saveUnits = (newUnits) => {
		setUnits(newUnits);
		try {
			localStorage.setItem("hanoi_military_units_data", JSON.stringify(newUnits));
		} catch (e) {
			console.warn("Could not save to localStorage", e);
		}
	};
	// Add / Edit Unit Handler
	const handleSaveUnit = (unit) => {
		const exists = units.some((u) => u.id === unit.id);
		let updated;
		if (exists) {
			updated = units.map((u) => u.id === unit.id ? unit : u);
		} else {
			updated = [unit, ...units];
		}
		saveUnits(updated);
		if (selectedUnit?.id === unit.id) {
			setSelectedUnit(unit);
		}
	};
	// Delete Unit Handler
	const handleDeleteUnit = (id) => {
		const updated = units.filter((u) => u.id !== id);
		saveUnits(updated);
		if (selectedUnit?.id === id) {
			setSelectedUnit(null);
		}
	};
	// Reset to initial seed data
	const handleResetSeedData = () => {
		if (confirm("Bạn có chắc muốn nạp lại dữ liệu chuẩn ban đầu của các đơn vị quân đội Hà Nội?")) {
			saveUnits(INITIAL_MILITARY_UNITS);
			setSelectedUnit(null);
			setBufferConfig({
				active: false,
				centerLat: 0,
				centerLng: 0,
				radiusKm: 10
			});
		}
	};
	// Import custom units
	const handleImportUnits = (importedUnits) => {
		const combined = [...importedUnits, ...units.filter((u) => !importedUnits.some((i) => i.id === u.id))];
		saveUnits(combined);
	};
	// Add intentional test anomaly to test data quality engine
	const handleAddTestAnomaly = () => {
		const testAnomalyUnit = {
			id: `DV_TEST_${Date.now()}`,
			code: "TEST-ERR-01",
			name: "Điểm thử nghiệm lỗi tọa độ (Ngoại vi Hà Giang)",
			shortName: "Điểm test lỗi",
			unitType: "Kho - Trạm - Căn cứ",
			branch: "Lục quân",
			parentUnit: "Bộ Tư lệnh Thủ đô Hà Nội",
			district: "Huyện Sóc Sơn",
			ward: "Thị trấn Sóc Sơn",
			address: "Vị trí lỗi cố tình ngoài ranh giới Hà Nội",
			latitude: 22.8234,
			longitude: 104.9812,
			x_vn2000: 498e3,
			y_vn2000: 2524e3,
			commander: "",
			phone: "",
			securityLevel: "Mật",
			readinessStatus: "Cấp 3 (Thường xuyên)",
			personnelScale: "Bản ghi kiểm thử",
			mainMissions: ["Kiểm tra thuật toán Topology & Bounding Box"],
			equipmentHighlights: [],
			establishedYear: 2026,
			updatedAt: new Date().toISOString()
		};
		saveUnits([testAnomalyUnit, ...units]);
		alert("Đã thêm 1 bản ghi thử nghiệm lỗi (Tọa độ ngoài Hà Nội & Thiếu thông tin) vào CSDL!");
	};
	// Quick search from Header
	const handleQuickSearch = (query) => {
		if (!query.trim()) {
			setActiveTab("map");
			return;
		}
		// Switch to search view with this keyword
		setActiveTab("search");
	};
	// Spatial audit count
	const topologyIssues = useMemo(() => auditMilitaryData(units), [units]);
	// Download Full Project ZIP
	const handleDownloadZip = () => {
		exportFullProjectZip(units);
	};
	return /* @__PURE__ */ _jsxDEV("div", {
		className: "flex flex-col h-screen w-screen overflow-hidden bg-[#070c16] text-white",
		children: [
			/* @__PURE__ */ _jsxDEV(Header, {
				onQuickSearch: handleQuickSearch,
				onOpenAddModal: () => {
					setEditingUnit(null);
					setIsAddEditModalOpen(true);
				},
				onOpenPostgisModal: () => setIsPostgisModalOpen(true),
				onDownloadZip: handleDownloadZip,
				unitsCount: units.length,
				postgisConnected: true
			}, void 0, false, {
				fileName: _jsxFileName,
				lineNumber: 170,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "flex flex-1 overflow-hidden relative",
				children: [/* @__PURE__ */ _jsxDEV(Sidebar, {
					activeTab,
					onSelectTab: (tab) => {
						setActiveTab(tab);
						if (tab !== "map") {}
					},
					unitsCount: units.length,
					issuesCount: topologyIssues.length
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 185,
					columnNumber: 9
				}, this), /* @__PURE__ */ _jsxDEV("main", {
					className: "flex-1 relative overflow-hidden flex flex-col",
					children: [
						activeTab === "overview" && /* @__PURE__ */ _jsxDEV(DashboardView, {
							units,
							onNavigateToMap: () => setActiveTab("map"),
							onFilterBranch: (branch) => setFilterBranch(branch),
							onSelectUnit: (unit) => setSelectedUnit(unit)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 201,
							columnNumber: 13
						}, this),
						activeTab === "map" && /* @__PURE__ */ _jsxDEV("div", {
							className: "relative w-full h-full flex-1",
							children: [/* @__PURE__ */ _jsxDEV(GisMap, {
								units,
								selectedUnit,
								onSelectUnit: (unit) => setSelectedUnit(unit),
								onUpdateCursorCoord: (coord) => setCursorCoord(coord),
								bufferConfig,
								onSetBufferConfig: (cfg) => setBufferConfig(cfg),
								filterBranch
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 212,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV(UnitDetailDrawer, {
								unit: selectedUnit,
								onClose: () => setSelectedUnit(null),
								onEdit: (unit) => {
									setEditingUnit(unit);
									setIsAddEditModalOpen(true);
								},
								onTriggerBuffer: (unit) => {
									setBufferConfig({
										active: true,
										centerLat: unit.latitude,
										centerLng: unit.longitude,
										radiusKm: 10,
										targetUnitName: unit.name
									});
								},
								onCenterMap: (unit) => {
									// Already handled via selectedUnit in GisMap
								}
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 223,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 211,
							columnNumber: 13
						}, this),
						activeTab === "search" && /* @__PURE__ */ _jsxDEV(UnitSearchView, {
							units,
							onSelectUnitAndGoToMap: (unit) => {
								setSelectedUnit(unit);
								setActiveTab("map");
							},
							onTriggerBuffer: (unit) => {
								setBufferConfig({
									active: true,
									centerLat: unit.latitude,
									centerLng: unit.longitude,
									radiusKm: 10,
									targetUnitName: unit.name
								});
								setSelectedUnit(unit);
								setActiveTab("map");
							}
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 248,
							columnNumber: 13
						}, this),
						activeTab === "management" && /* @__PURE__ */ _jsxDEV(DataManagementView, {
							units,
							onOpenAddModal: () => {
								setEditingUnit(null);
								setIsAddEditModalOpen(true);
							},
							onEditUnit: (unit) => {
								setEditingUnit(unit);
								setIsAddEditModalOpen(true);
							},
							onDeleteUnit: handleDeleteUnit,
							onResetSeedData: handleResetSeedData,
							onImportUnits: handleImportUnits,
							onDownloadZip: handleDownloadZip
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 270,
							columnNumber: 13
						}, this),
						activeTab === "spatial" && /* @__PURE__ */ _jsxDEV(SpatialAnalysisView, {
							units,
							onApplyBufferAndGoToMap: (cfg) => {
								setBufferConfig(cfg);
								setActiveTab("map");
							},
							onSelectUnitAndGoToMap: (unit) => {
								setSelectedUnit(unit);
								setActiveTab("map");
							}
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 289,
							columnNumber: 13
						}, this),
						activeTab === "statistics" && /* @__PURE__ */ _jsxDEV(StatisticsView, { units }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 303,
							columnNumber: 42
						}, this),
						activeTab === "checker" && /* @__PURE__ */ _jsxDEV(DataCheckerView, {
							units,
							onEditUnit: (unit) => {
								setEditingUnit(unit);
								setIsAddEditModalOpen(true);
							},
							onSelectUnitAndGoToMap: (unit) => {
								setSelectedUnit(unit);
								setActiveTab("map");
							},
							onAddTestAnomaly: handleAddTestAnomaly
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 307,
							columnNumber: 13
						}, this)
					]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 198,
					columnNumber: 9
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 183,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV(StatusBar, {
				cursorCoord,
				activeLayerName: activeTab === "map" ? filterBranch ? `Lọc: ${filterBranch}` : "Toàn bộ 30 Quận Huyện Hà Nội" : activeTab.toUpperCase(),
				totalUnitsCount: units.length,
				basemapName: "Vệ tinh Esri / Bản đồ số VN-2000"
			}, void 0, false, {
				fileName: _jsxFileName,
				lineNumber: 324,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV(UnitModal, {
				isOpen: isAddEditModalOpen,
				onClose: () => {
					setIsAddEditModalOpen(false);
					setEditingUnit(null);
				},
				onSave: handleSaveUnit,
				initialUnit: editingUnit
			}, void 0, false, {
				fileName: _jsxFileName,
				lineNumber: 338,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV(PostgisModal, {
				isOpen: isPostgisModalOpen,
				onClose: () => setIsPostgisModalOpen(false),
				units
			}, void 0, false, {
				fileName: _jsxFileName,
				lineNumber: 349,
				columnNumber: 7
			}, this)
		]
	}, void 0, true, {
		fileName: _jsxFileName,
		lineNumber: 168,
		columnNumber: 5
	}, this);
}

//# sourceMappingURL=data:application/json;base64,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dXJjZXNDb250ZW50IjpbIi8qKlxuICogQGxpY2Vuc2VcbiAqIFNQRFgtTGljZW5zZS1JZGVudGlmaWVyOiBBcGFjaGUtMi4wXG4gKi9cblxuaW1wb3J0IFJlYWN0LCB7IHVzZVN0YXRlLCB1c2VNZW1vIH0gZnJvbSAncmVhY3QnO1xuaW1wb3J0IHsgTWlsaXRhcnlVbml0LCBCdWZmZXJBbmFseXNpcyB9IGZyb20gJy4vdHlwZXMvZ2lzJztcbmltcG9ydCB7IElOSVRJQUxfTUlMSVRBUllfVU5JVFMgfSBmcm9tICcuL2RhdGEvbWlsaXRhcnlVbml0cyc7XG5pbXBvcnQgeyBhdWRpdE1pbGl0YXJ5RGF0YSB9IGZyb20gJy4vdXRpbHMvZ2lzQ2FsY3VsYXRpb25zJztcbmltcG9ydCB7IGV4cG9ydEZ1bGxQcm9qZWN0WmlwIH0gZnJvbSAnLi91dGlscy9wcm9qZWN0WmlwRXhwb3J0ZXInO1xuaW1wb3J0IHsgSGVhZGVyIH0gZnJvbSAnLi9jb21wb25lbnRzL0hlYWRlcic7XG5pbXBvcnQgeyBTaWRlYmFyLCBUYWJUeXBlIH0gZnJvbSAnLi9jb21wb25lbnRzL1NpZGViYXInO1xuaW1wb3J0IHsgU3RhdHVzQmFyIH0gZnJvbSAnLi9jb21wb25lbnRzL1N0YXR1c0Jhcic7XG5pbXBvcnQgeyBHaXNNYXAgfSBmcm9tICcuL2NvbXBvbmVudHMvR2lzTWFwJztcbmltcG9ydCB7IFVuaXREZXRhaWxEcmF3ZXIgfSBmcm9tICcuL2NvbXBvbmVudHMvVW5pdERldGFpbERyYXdlcic7XG5pbXBvcnQgeyBEYXNoYm9hcmRWaWV3IH0gZnJvbSAnLi9jb21wb25lbnRzL0Rhc2hib2FyZFZpZXcnO1xuaW1wb3J0IHsgVW5pdFNlYXJjaFZpZXcgfSBmcm9tICcuL2NvbXBvbmVudHMvVW5pdFNlYXJjaFZpZXcnO1xuaW1wb3J0IHsgRGF0YU1hbmFnZW1lbnRWaWV3IH0gZnJvbSAnLi9jb21wb25lbnRzL0RhdGFNYW5hZ2VtZW50Vmlldyc7XG5pbXBvcnQgeyBTcGF0aWFsQW5hbHlzaXNWaWV3IH0gZnJvbSAnLi9jb21wb25lbnRzL1NwYXRpYWxBbmFseXNpc1ZpZXcnO1xuaW1wb3J0IHsgU3RhdGlzdGljc1ZpZXcgfSBmcm9tICcuL2NvbXBvbmVudHMvU3RhdGlzdGljc1ZpZXcnO1xuaW1wb3J0IHsgRGF0YUNoZWNrZXJWaWV3IH0gZnJvbSAnLi9jb21wb25lbnRzL0RhdGFDaGVja2VyVmlldyc7XG5pbXBvcnQgeyBVbml0TW9kYWwgfSBmcm9tICcuL2NvbXBvbmVudHMvVW5pdE1vZGFsJztcbmltcG9ydCB7IFBvc3RnaXNNb2RhbCB9IGZyb20gJy4vY29tcG9uZW50cy9Qb3N0Z2lzTW9kYWwnO1xuXG5leHBvcnQgZGVmYXVsdCBmdW5jdGlvbiBBcHAoKSB7XG4gIGNvbnN0IFt1bml0cywgc2V0VW5pdHNdID0gdXNlU3RhdGU8TWlsaXRhcnlVbml0W10+KCgpID0+IHtcbiAgICBjb25zdCBzYXZlZCA9IGxvY2FsU3RvcmFnZS5nZXRJdGVtKCdoYW5vaV9taWxpdGFyeV91bml0c19kYXRhJyk7XG4gICAgaWYgKHNhdmVkKSB7XG4gICAgICB0cnkge1xuICAgICAgICBjb25zdCBwYXJzZWQgPSBKU09OLnBhcnNlKHNhdmVkKTtcbiAgICAgICAgaWYgKEFycmF5LmlzQXJyYXkocGFyc2VkKSAmJiBwYXJzZWQubGVuZ3RoID4gMCkgcmV0dXJuIHBhcnNlZDtcbiAgICAgIH0gY2F0Y2ggKGUpIHtcbiAgICAgICAgLy8gZmFsbGJhY2sgdG8gZGVmYXVsdFxuICAgICAgfVxuICAgIH1cbiAgICByZXR1cm4gSU5JVElBTF9NSUxJVEFSWV9VTklUUztcbiAgfSk7XG5cbiAgLy8gQ3VycmVudCBhY3RpdmUgdGFiIChkZWZhdWx0IHRvICdtYXAnIHRvIG1hdGNoIHVzZXIncyBpbWFnZS5wbmcpXG4gIGNvbnN0IFthY3RpdmVUYWIsIHNldEFjdGl2ZVRhYl0gPSB1c2VTdGF0ZTxUYWJUeXBlPignbWFwJyk7XG5cbiAgLy8gQ3VycmVudGx5IGluc3BlY3RlZCBtaWxpdGFyeSB1bml0XG4gIGNvbnN0IFtzZWxlY3RlZFVuaXQsIHNldFNlbGVjdGVkVW5pdF0gPSB1c2VTdGF0ZTxNaWxpdGFyeVVuaXQgfCBudWxsPihudWxsKTtcblxuICAvLyBMaXZlIGN1cnNvciBjb29yZGluYXRlcyBmcm9tIExlYWZsZXQgbWFwXG4gIGNvbnN0IFtjdXJzb3JDb29yZCwgc2V0Q3Vyc29yQ29vcmRdID0gdXNlU3RhdGU8e1xuICAgIGxhdDogbnVtYmVyO1xuICAgIGxuZzogbnVtYmVyO1xuICAgIHg6IG51bWJlcjtcbiAgICB5OiBudW1iZXI7XG4gIH0gfCBudWxsPihudWxsKTtcblxuICAvLyBCdWZmZXIgYW5hbHlzaXMgc3RhdGVcbiAgY29uc3QgW2J1ZmZlckNvbmZpZywgc2V0QnVmZmVyQ29uZmlnXSA9IHVzZVN0YXRlPEJ1ZmZlckFuYWx5c2lzPih7XG4gICAgYWN0aXZlOiBmYWxzZSxcbiAgICBjZW50ZXJMYXQ6IDAsXG4gICAgY2VudGVyTG5nOiAwLFxuICAgIHJhZGl1c0ttOiAxMCxcbiAgfSk7XG5cbiAgLy8gRmlsdGVyc1xuICBjb25zdCBbZmlsdGVyQnJhbmNoLCBzZXRGaWx0ZXJCcmFuY2hdID0gdXNlU3RhdGU8c3RyaW5nIHwgdW5kZWZpbmVkPih1bmRlZmluZWQpO1xuXG4gIC8vIE1vZGFsc1xuICBjb25zdCBbaXNBZGRFZGl0TW9kYWxPcGVuLCBzZXRJc0FkZEVkaXRNb2RhbE9wZW5dID0gdXNlU3RhdGUoZmFsc2UpO1xuICBjb25zdCBbZWRpdGluZ1VuaXQsIHNldEVkaXRpbmdVbml0XSA9IHVzZVN0YXRlPE1pbGl0YXJ5VW5pdCB8IG51bGw+KG51bGwpO1xuICBjb25zdCBbaXNQb3N0Z2lzTW9kYWxPcGVuLCBzZXRJc1Bvc3RnaXNNb2RhbE9wZW5dID0gdXNlU3RhdGUoZmFsc2UpO1xuXG4gIC8vIFNhdmUgdG8gbG9jYWxTdG9yYWdlIHdoZW5ldmVyIHVuaXRzIHVwZGF0ZVxuICBjb25zdCBzYXZlVW5pdHMgPSAobmV3VW5pdHM6IE1pbGl0YXJ5VW5pdFtdKSA9PiB7XG4gICAgc2V0VW5pdHMobmV3VW5pdHMpO1xuICAgIHRyeSB7XG4gICAgICBsb2NhbFN0b3JhZ2Uuc2V0SXRlbSgnaGFub2lfbWlsaXRhcnlfdW5pdHNfZGF0YScsIEpTT04uc3RyaW5naWZ5KG5ld1VuaXRzKSk7XG4gICAgfSBjYXRjaCAoZSkge1xuICAgICAgY29uc29sZS53YXJuKCdDb3VsZCBub3Qgc2F2ZSB0byBsb2NhbFN0b3JhZ2UnLCBlKTtcbiAgICB9XG4gIH07XG5cbiAgLy8gQWRkIC8gRWRpdCBVbml0IEhhbmRsZXJcbiAgY29uc3QgaGFuZGxlU2F2ZVVuaXQgPSAodW5pdDogTWlsaXRhcnlVbml0KSA9PiB7XG4gICAgY29uc3QgZXhpc3RzID0gdW5pdHMuc29tZSgodSkgPT4gdS5pZCA9PT0gdW5pdC5pZCk7XG4gICAgbGV0IHVwZGF0ZWQ6IE1pbGl0YXJ5VW5pdFtdO1xuICAgIGlmIChleGlzdHMpIHtcbiAgICAgIHVwZGF0ZWQgPSB1bml0cy5tYXAoKHUpID0+ICh1LmlkID09PSB1bml0LmlkID8gdW5pdCA6IHUpKTtcbiAgICB9IGVsc2Uge1xuICAgICAgdXBkYXRlZCA9IFt1bml0LCAuLi51bml0c107XG4gICAgfVxuICAgIHNhdmVVbml0cyh1cGRhdGVkKTtcbiAgICBpZiAoc2VsZWN0ZWRVbml0Py5pZCA9PT0gdW5pdC5pZCkge1xuICAgICAgc2V0U2VsZWN0ZWRVbml0KHVuaXQpO1xuICAgIH1cbiAgfTtcblxuICAvLyBEZWxldGUgVW5pdCBIYW5kbGVyXG4gIGNvbnN0IGhhbmRsZURlbGV0ZVVuaXQgPSAoaWQ6IHN0cmluZykgPT4ge1xuICAgIGNvbnN0IHVwZGF0ZWQgPSB1bml0cy5maWx0ZXIoKHUpID0+IHUuaWQgIT09IGlkKTtcbiAgICBzYXZlVW5pdHModXBkYXRlZCk7XG4gICAgaWYgKHNlbGVjdGVkVW5pdD8uaWQgPT09IGlkKSB7XG4gICAgICBzZXRTZWxlY3RlZFVuaXQobnVsbCk7XG4gICAgfVxuICB9O1xuXG4gIC8vIFJlc2V0IHRvIGluaXRpYWwgc2VlZCBkYXRhXG4gIGNvbnN0IGhhbmRsZVJlc2V0U2VlZERhdGEgPSAoKSA9PiB7XG4gICAgaWYgKGNvbmZpcm0oJ0LhuqFuIGPDsyBjaOG6r2MgbXXhu5FuIG7huqFwIGzhuqFpIGThu68gbGnhu4d1IGNodeG6qW4gYmFuIMSR4bqndSBj4bunYSBjw6FjIMSRxqFuIHbhu4sgcXXDom4gxJHhu5lpIEjDoCBO4buZaT8nKSkge1xuICAgICAgc2F2ZVVuaXRzKElOSVRJQUxfTUlMSVRBUllfVU5JVFMpO1xuICAgICAgc2V0U2VsZWN0ZWRVbml0KG51bGwpO1xuICAgICAgc2V0QnVmZmVyQ29uZmlnKHsgYWN0aXZlOiBmYWxzZSwgY2VudGVyTGF0OiAwLCBjZW50ZXJMbmc6IDAsIHJhZGl1c0ttOiAxMCB9KTtcbiAgICB9XG4gIH07XG5cbiAgLy8gSW1wb3J0IGN1c3RvbSB1bml0c1xuICBjb25zdCBoYW5kbGVJbXBvcnRVbml0cyA9IChpbXBvcnRlZFVuaXRzOiBNaWxpdGFyeVVuaXRbXSkgPT4ge1xuICAgIGNvbnN0IGNvbWJpbmVkID0gWy4uLmltcG9ydGVkVW5pdHMsIC4uLnVuaXRzLmZpbHRlcigodSkgPT4gIWltcG9ydGVkVW5pdHMuc29tZSgoaSkgPT4gaS5pZCA9PT0gdS5pZCkpXTtcbiAgICBzYXZlVW5pdHMoY29tYmluZWQpO1xuICB9O1xuXG4gIC8vIEFkZCBpbnRlbnRpb25hbCB0ZXN0IGFub21hbHkgdG8gdGVzdCBkYXRhIHF1YWxpdHkgZW5naW5lXG4gIGNvbnN0IGhhbmRsZUFkZFRlc3RBbm9tYWx5ID0gKCkgPT4ge1xuICAgIGNvbnN0IHRlc3RBbm9tYWx5VW5pdDogTWlsaXRhcnlVbml0ID0ge1xuICAgICAgaWQ6IGBEVl9URVNUXyR7RGF0ZS5ub3coKX1gLFxuICAgICAgY29kZTogJ1RFU1QtRVJSLTAxJyxcbiAgICAgIG5hbWU6ICfEkGnhu4NtIHRo4butIG5naGnhu4dtIGzhu5dpIHThu41hIMSR4buZIChOZ2/huqFpIHZpIEjDoCBHaWFuZyknLFxuICAgICAgc2hvcnROYW1lOiAnxJBp4buDbSB0ZXN0IGzhu5dpJyxcbiAgICAgIHVuaXRUeXBlOiAnS2hvIC0gVHLhuqFtIC0gQ8SDbiBj4bupJyxcbiAgICAgIGJyYW5jaDogJ0zhu6VjIHF1w6JuJyxcbiAgICAgIHBhcmVudFVuaXQ6ICdC4buZIFTGsCBs4buHbmggVGjhu6cgxJHDtCBIw6AgTuG7mWknLFxuICAgICAgZGlzdHJpY3Q6ICdIdXnhu4duIFPDs2MgU8ahbicsXG4gICAgICB3YXJkOiAnVGjhu4sgdHLhuqVuIFPDs2MgU8ahbicsXG4gICAgICBhZGRyZXNzOiAnVuG7iyB0csOtIGzhu5dpIGPhu5EgdMOsbmggbmdvw6BpIHJhbmggZ2nhu5tpIEjDoCBO4buZaScsXG4gICAgICBsYXRpdHVkZTogMjIuODIzNCwgLy8gT3V0c2lkZSBIYW5vaVxuICAgICAgbG9uZ2l0dWRlOiAxMDQuOTgxMiwgLy8gT3V0c2lkZSBIYW5vaVxuICAgICAgeF92bjIwMDA6IDQ5ODAwMCxcbiAgICAgIHlfdm4yMDAwOiAyNTI0MDAwLFxuICAgICAgY29tbWFuZGVyOiAnJywgLy8gTWlzc2luZyBjb21tYW5kZXIgdG8gdHJpZ2dlciB3YXJuaW5nXG4gICAgICBwaG9uZTogJycsIC8vIE1pc3NpbmcgcGhvbmUgdG8gdHJpZ2dlciB3YXJuaW5nXG4gICAgICBzZWN1cml0eUxldmVsOiAnTeG6rXQnLFxuICAgICAgcmVhZGluZXNzU3RhdHVzOiAnQ+G6pXAgMyAoVGjGsOG7nW5nIHh1ecOqbiknLFxuICAgICAgcGVyc29ubmVsU2NhbGU6ICdC4bqjbiBnaGkga2nhu4NtIHRo4butJyxcbiAgICAgIG1haW5NaXNzaW9uczogWydLaeG7g20gdHJhIHRodeG6rXQgdG/DoW4gVG9wb2xvZ3kgJiBCb3VuZGluZyBCb3gnXSxcbiAgICAgIGVxdWlwbWVudEhpZ2hsaWdodHM6IFtdLFxuICAgICAgZXN0YWJsaXNoZWRZZWFyOiAyMDI2LFxuICAgICAgdXBkYXRlZEF0OiBuZXcgRGF0ZSgpLnRvSVNPU3RyaW5nKCksXG4gICAgfTtcbiAgICBzYXZlVW5pdHMoW3Rlc3RBbm9tYWx5VW5pdCwgLi4udW5pdHNdKTtcbiAgICBhbGVydCgnxJDDoyB0aMOqbSAxIGLhuqNuIGdoaSB0aOG7rSBuZ2hp4buHbSBs4buXaSAoVOG7jWEgxJHhu5kgbmdvw6BpIEjDoCBO4buZaSAmIFRoaeG6v3UgdGjDtG5nIHRpbikgdsOgbyBDU0RMIScpO1xuICB9O1xuXG4gIC8vIFF1aWNrIHNlYXJjaCBmcm9tIEhlYWRlclxuICBjb25zdCBoYW5kbGVRdWlja1NlYXJjaCA9IChxdWVyeTogc3RyaW5nKSA9PiB7XG4gICAgaWYgKCFxdWVyeS50cmltKCkpIHtcbiAgICAgIHNldEFjdGl2ZVRhYignbWFwJyk7XG4gICAgICByZXR1cm47XG4gICAgfVxuICAgIC8vIFN3aXRjaCB0byBzZWFyY2ggdmlldyB3aXRoIHRoaXMga2V5d29yZFxuICAgIHNldEFjdGl2ZVRhYignc2VhcmNoJyk7XG4gIH07XG5cbiAgLy8gU3BhdGlhbCBhdWRpdCBjb3VudFxuICBjb25zdCB0b3BvbG9neUlzc3VlcyA9IHVzZU1lbW8oKCkgPT4gYXVkaXRNaWxpdGFyeURhdGEodW5pdHMpLCBbdW5pdHNdKTtcblxuICAvLyBEb3dubG9hZCBGdWxsIFByb2plY3QgWklQXG4gIGNvbnN0IGhhbmRsZURvd25sb2FkWmlwID0gKCkgPT4ge1xuICAgIGV4cG9ydEZ1bGxQcm9qZWN0WmlwKHVuaXRzKTtcbiAgfTtcblxuICByZXR1cm4gKFxuICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBmbGV4LWNvbCBoLXNjcmVlbiB3LXNjcmVlbiBvdmVyZmxvdy1oaWRkZW4gYmctWyMwNzBjMTZdIHRleHQtd2hpdGVcIj5cbiAgICAgIHsvKiAxLiBUb3AgSGVhZGVyIChFbWJsZW0sIFRpdGxlLCBRdWljayBTZWFyY2gsIFBvc3RHSVMgc3RhdHVzLCBBZGQgVW5pdCwgRG93bmxvYWQgWmlwKSAqL31cbiAgICAgIDxIZWFkZXJcbiAgICAgICAgb25RdWlja1NlYXJjaD17aGFuZGxlUXVpY2tTZWFyY2h9XG4gICAgICAgIG9uT3BlbkFkZE1vZGFsPXsoKSA9PiB7XG4gICAgICAgICAgc2V0RWRpdGluZ1VuaXQobnVsbCk7XG4gICAgICAgICAgc2V0SXNBZGRFZGl0TW9kYWxPcGVuKHRydWUpO1xuICAgICAgICB9fVxuICAgICAgICBvbk9wZW5Qb3N0Z2lzTW9kYWw9eygpID0+IHNldElzUG9zdGdpc01vZGFsT3Blbih0cnVlKX1cbiAgICAgICAgb25Eb3dubG9hZFppcD17aGFuZGxlRG93bmxvYWRaaXB9XG4gICAgICAgIHVuaXRzQ291bnQ9e3VuaXRzLmxlbmd0aH1cbiAgICAgICAgcG9zdGdpc0Nvbm5lY3RlZD17dHJ1ZX1cbiAgICAgIC8+XG5cbiAgICAgIHsvKiAyLiBNYWluIE1pZGRsZSBXb3Jrc3BhY2U6IFNpZGViYXIgKyBDZW50cmFsIENvbnRlbnQgKi99XG4gICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggZmxleC0xIG92ZXJmbG93LWhpZGRlbiByZWxhdGl2ZVwiPlxuICAgICAgICB7LyogTGVmdCBTaWRlYmFyIChNYXRjaGluZyBpbWFnZS5wbmc6IFThu5VuZyBxdWFuLCBC4bqjbiDEkeG7kyBHSVMsIFRyYSBj4bupdSwgUXXhuqNuIGzDvSwgUGjDom4gdMOtY2gsIFRo4buRbmcga8OqLCBLaeG7g20gdHJhKSAqL31cbiAgICAgICAgPFNpZGViYXJcbiAgICAgICAgICBhY3RpdmVUYWI9e2FjdGl2ZVRhYn1cbiAgICAgICAgICBvblNlbGVjdFRhYj17KHRhYikgPT4ge1xuICAgICAgICAgICAgc2V0QWN0aXZlVGFiKHRhYik7XG4gICAgICAgICAgICBpZiAodGFiICE9PSAnbWFwJykge1xuICAgICAgICAgICAgICAvLyBrZWVwIHVuaXQgb3IgcmVzZXRcbiAgICAgICAgICAgIH1cbiAgICAgICAgICB9fVxuICAgICAgICAgIHVuaXRzQ291bnQ9e3VuaXRzLmxlbmd0aH1cbiAgICAgICAgICBpc3N1ZXNDb3VudD17dG9wb2xvZ3lJc3N1ZXMubGVuZ3RofVxuICAgICAgICAvPlxuXG4gICAgICAgIHsvKiBDZW50cmFsIFZpZXcgQ29udGFpbmVyICovfVxuICAgICAgICA8bWFpbiBjbGFzc05hbWU9XCJmbGV4LTEgcmVsYXRpdmUgb3ZlcmZsb3ctaGlkZGVuIGZsZXggZmxleC1jb2xcIj5cbiAgICAgICAgICB7LyogVEFCIDE6IE9WRVJWSUVXICovfVxuICAgICAgICAgIHthY3RpdmVUYWIgPT09ICdvdmVydmlldycgJiYgKFxuICAgICAgICAgICAgPERhc2hib2FyZFZpZXdcbiAgICAgICAgICAgICAgdW5pdHM9e3VuaXRzfVxuICAgICAgICAgICAgICBvbk5hdmlnYXRlVG9NYXA9eygpID0+IHNldEFjdGl2ZVRhYignbWFwJyl9XG4gICAgICAgICAgICAgIG9uRmlsdGVyQnJhbmNoPXsoYnJhbmNoKSA9PiBzZXRGaWx0ZXJCcmFuY2goYnJhbmNoKX1cbiAgICAgICAgICAgICAgb25TZWxlY3RVbml0PXsodW5pdCkgPT4gc2V0U2VsZWN0ZWRVbml0KHVuaXQpfVxuICAgICAgICAgICAgLz5cbiAgICAgICAgICApfVxuXG4gICAgICAgICAgey8qIFRBQiAyOiBHSVMgTUFQIChMYXlvdXQgbWF0Y2hlcyBpbWFnZS5wbmcpICovfVxuICAgICAgICAgIHthY3RpdmVUYWIgPT09ICdtYXAnICYmIChcbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwicmVsYXRpdmUgdy1mdWxsIGgtZnVsbCBmbGV4LTFcIj5cbiAgICAgICAgICAgICAgPEdpc01hcFxuICAgICAgICAgICAgICAgIHVuaXRzPXt1bml0c31cbiAgICAgICAgICAgICAgICBzZWxlY3RlZFVuaXQ9e3NlbGVjdGVkVW5pdH1cbiAgICAgICAgICAgICAgICBvblNlbGVjdFVuaXQ9eyh1bml0KSA9PiBzZXRTZWxlY3RlZFVuaXQodW5pdCl9XG4gICAgICAgICAgICAgICAgb25VcGRhdGVDdXJzb3JDb29yZD17KGNvb3JkKSA9PiBzZXRDdXJzb3JDb29yZChjb29yZCl9XG4gICAgICAgICAgICAgICAgYnVmZmVyQ29uZmlnPXtidWZmZXJDb25maWd9XG4gICAgICAgICAgICAgICAgb25TZXRCdWZmZXJDb25maWc9eyhjZmcpID0+IHNldEJ1ZmZlckNvbmZpZyhjZmcpfVxuICAgICAgICAgICAgICAgIGZpbHRlckJyYW5jaD17ZmlsdGVyQnJhbmNofVxuICAgICAgICAgICAgICAvPlxuXG4gICAgICAgICAgICAgIHsvKiBVbml0IERldGFpbCBEcmF3ZXIgaWYgYSB1bml0IGlzIHNlbGVjdGVkICovfVxuICAgICAgICAgICAgICA8VW5pdERldGFpbERyYXdlclxuICAgICAgICAgICAgICAgIHVuaXQ9e3NlbGVjdGVkVW5pdH1cbiAgICAgICAgICAgICAgICBvbkNsb3NlPXsoKSA9PiBzZXRTZWxlY3RlZFVuaXQobnVsbCl9XG4gICAgICAgICAgICAgICAgb25FZGl0PXsodW5pdCkgPT4ge1xuICAgICAgICAgICAgICAgICAgc2V0RWRpdGluZ1VuaXQodW5pdCk7XG4gICAgICAgICAgICAgICAgICBzZXRJc0FkZEVkaXRNb2RhbE9wZW4odHJ1ZSk7XG4gICAgICAgICAgICAgICAgfX1cbiAgICAgICAgICAgICAgICBvblRyaWdnZXJCdWZmZXI9eyh1bml0KSA9PiB7XG4gICAgICAgICAgICAgICAgICBzZXRCdWZmZXJDb25maWcoe1xuICAgICAgICAgICAgICAgICAgICBhY3RpdmU6IHRydWUsXG4gICAgICAgICAgICAgICAgICAgIGNlbnRlckxhdDogdW5pdC5sYXRpdHVkZSxcbiAgICAgICAgICAgICAgICAgICAgY2VudGVyTG5nOiB1bml0LmxvbmdpdHVkZSxcbiAgICAgICAgICAgICAgICAgICAgcmFkaXVzS206IDEwLFxuICAgICAgICAgICAgICAgICAgICB0YXJnZXRVbml0TmFtZTogdW5pdC5uYW1lLFxuICAgICAgICAgICAgICAgICAgfSk7XG4gICAgICAgICAgICAgICAgfX1cbiAgICAgICAgICAgICAgICBvbkNlbnRlck1hcD17KHVuaXQpID0+IHtcbiAgICAgICAgICAgICAgICAgIC8vIEFscmVhZHkgaGFuZGxlZCB2aWEgc2VsZWN0ZWRVbml0IGluIEdpc01hcFxuICAgICAgICAgICAgICAgIH19XG4gICAgICAgICAgICAgIC8+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICApfVxuXG4gICAgICAgICAgey8qIFRBQiAzOiBVTklUIFNFQVJDSCAqL31cbiAgICAgICAgICB7YWN0aXZlVGFiID09PSAnc2VhcmNoJyAmJiAoXG4gICAgICAgICAgICA8VW5pdFNlYXJjaFZpZXdcbiAgICAgICAgICAgICAgdW5pdHM9e3VuaXRzfVxuICAgICAgICAgICAgICBvblNlbGVjdFVuaXRBbmRHb1RvTWFwPXsodW5pdCkgPT4ge1xuICAgICAgICAgICAgICAgIHNldFNlbGVjdGVkVW5pdCh1bml0KTtcbiAgICAgICAgICAgICAgICBzZXRBY3RpdmVUYWIoJ21hcCcpO1xuICAgICAgICAgICAgICB9fVxuICAgICAgICAgICAgICBvblRyaWdnZXJCdWZmZXI9eyh1bml0KSA9PiB7XG4gICAgICAgICAgICAgICAgc2V0QnVmZmVyQ29uZmlnKHtcbiAgICAgICAgICAgICAgICAgIGFjdGl2ZTogdHJ1ZSxcbiAgICAgICAgICAgICAgICAgIGNlbnRlckxhdDogdW5pdC5sYXRpdHVkZSxcbiAgICAgICAgICAgICAgICAgIGNlbnRlckxuZzogdW5pdC5sb25naXR1ZGUsXG4gICAgICAgICAgICAgICAgICByYWRpdXNLbTogMTAsXG4gICAgICAgICAgICAgICAgICB0YXJnZXRVbml0TmFtZTogdW5pdC5uYW1lLFxuICAgICAgICAgICAgICAgIH0pO1xuICAgICAgICAgICAgICAgIHNldFNlbGVjdGVkVW5pdCh1bml0KTtcbiAgICAgICAgICAgICAgICBzZXRBY3RpdmVUYWIoJ21hcCcpO1xuICAgICAgICAgICAgICB9fVxuICAgICAgICAgICAgLz5cbiAgICAgICAgICApfVxuXG4gICAgICAgICAgey8qIFRBQiA0OiBEQVRBIE1BTkFHRU1FTlQgKENSVUQgKyBQb3N0R0lTIFNRTCArIEdlb0pTT04gKyBDU1YpICovfVxuICAgICAgICAgIHthY3RpdmVUYWIgPT09ICdtYW5hZ2VtZW50JyAmJiAoXG4gICAgICAgICAgICA8RGF0YU1hbmFnZW1lbnRWaWV3XG4gICAgICAgICAgICAgIHVuaXRzPXt1bml0c31cbiAgICAgICAgICAgICAgb25PcGVuQWRkTW9kYWw9eygpID0+IHtcbiAgICAgICAgICAgICAgICBzZXRFZGl0aW5nVW5pdChudWxsKTtcbiAgICAgICAgICAgICAgICBzZXRJc0FkZEVkaXRNb2RhbE9wZW4odHJ1ZSk7XG4gICAgICAgICAgICAgIH19XG4gICAgICAgICAgICAgIG9uRWRpdFVuaXQ9eyh1bml0KSA9PiB7XG4gICAgICAgICAgICAgICAgc2V0RWRpdGluZ1VuaXQodW5pdCk7XG4gICAgICAgICAgICAgICAgc2V0SXNBZGRFZGl0TW9kYWxPcGVuKHRydWUpO1xuICAgICAgICAgICAgICB9fVxuICAgICAgICAgICAgICBvbkRlbGV0ZVVuaXQ9e2hhbmRsZURlbGV0ZVVuaXR9XG4gICAgICAgICAgICAgIG9uUmVzZXRTZWVkRGF0YT17aGFuZGxlUmVzZXRTZWVkRGF0YX1cbiAgICAgICAgICAgICAgb25JbXBvcnRVbml0cz17aGFuZGxlSW1wb3J0VW5pdHN9XG4gICAgICAgICAgICAgIG9uRG93bmxvYWRaaXA9e2hhbmRsZURvd25sb2FkWmlwfVxuICAgICAgICAgICAgLz5cbiAgICAgICAgICApfVxuXG4gICAgICAgICAgey8qIFRBQiA1OiBTUEFUSUFMIEFOQUxZU0lTIChCdWZmZXIsIEstTk4sIERlZmVuc2l2ZSBTZWN0b3JzKSAqL31cbiAgICAgICAgICB7YWN0aXZlVGFiID09PSAnc3BhdGlhbCcgJiYgKFxuICAgICAgICAgICAgPFNwYXRpYWxBbmFseXNpc1ZpZXdcbiAgICAgICAgICAgICAgdW5pdHM9e3VuaXRzfVxuICAgICAgICAgICAgICBvbkFwcGx5QnVmZmVyQW5kR29Ub01hcD17KGNmZykgPT4ge1xuICAgICAgICAgICAgICAgIHNldEJ1ZmZlckNvbmZpZyhjZmcpO1xuICAgICAgICAgICAgICAgIHNldEFjdGl2ZVRhYignbWFwJyk7XG4gICAgICAgICAgICAgIH19XG4gICAgICAgICAgICAgIG9uU2VsZWN0VW5pdEFuZEdvVG9NYXA9eyh1bml0KSA9PiB7XG4gICAgICAgICAgICAgICAgc2V0U2VsZWN0ZWRVbml0KHVuaXQpO1xuICAgICAgICAgICAgICAgIHNldEFjdGl2ZVRhYignbWFwJyk7XG4gICAgICAgICAgICAgIH19XG4gICAgICAgICAgICAvPlxuICAgICAgICAgICl9XG5cbiAgICAgICAgICB7LyogVEFCIDY6IFNUQVRJU1RJQ1MgJiBSRVBPUlRJTkcgKi99XG4gICAgICAgICAge2FjdGl2ZVRhYiA9PT0gJ3N0YXRpc3RpY3MnICYmIDxTdGF0aXN0aWNzVmlldyB1bml0cz17dW5pdHN9IC8+fVxuXG4gICAgICAgICAgey8qIFRBQiA3OiBEQVRBICYgVE9QT0xPR1kgQ0hFQ0tFUiAqL31cbiAgICAgICAgICB7YWN0aXZlVGFiID09PSAnY2hlY2tlcicgJiYgKFxuICAgICAgICAgICAgPERhdGFDaGVja2VyVmlld1xuICAgICAgICAgICAgICB1bml0cz17dW5pdHN9XG4gICAgICAgICAgICAgIG9uRWRpdFVuaXQ9eyh1bml0KSA9PiB7XG4gICAgICAgICAgICAgICAgc2V0RWRpdGluZ1VuaXQodW5pdCk7XG4gICAgICAgICAgICAgICAgc2V0SXNBZGRFZGl0TW9kYWxPcGVuKHRydWUpO1xuICAgICAgICAgICAgICB9fVxuICAgICAgICAgICAgICBvblNlbGVjdFVuaXRBbmRHb1RvTWFwPXsodW5pdCkgPT4ge1xuICAgICAgICAgICAgICAgIHNldFNlbGVjdGVkVW5pdCh1bml0KTtcbiAgICAgICAgICAgICAgICBzZXRBY3RpdmVUYWIoJ21hcCcpO1xuICAgICAgICAgICAgICB9fVxuICAgICAgICAgICAgICBvbkFkZFRlc3RBbm9tYWx5PXtoYW5kbGVBZGRUZXN0QW5vbWFseX1cbiAgICAgICAgICAgIC8+XG4gICAgICAgICAgKX1cbiAgICAgICAgPC9tYWluPlxuICAgICAgPC9kaXY+XG5cbiAgICAgIHsvKiAzLiBCb3R0b20gU3RhdHVzIEJhciAoQ1JTIEVQU0c6MzI2NDgsIExpdmUgQ29vcmRpbmF0ZXMsIEFjdGl2ZSBMYXllcnMpICovfVxuICAgICAgPFN0YXR1c0JhclxuICAgICAgICBjdXJzb3JDb29yZD17Y3Vyc29yQ29vcmR9XG4gICAgICAgIGFjdGl2ZUxheWVyTmFtZT17XG4gICAgICAgICAgYWN0aXZlVGFiID09PSAnbWFwJ1xuICAgICAgICAgICAgPyBmaWx0ZXJCcmFuY2hcbiAgICAgICAgICAgICAgPyBgTOG7jWM6ICR7ZmlsdGVyQnJhbmNofWBcbiAgICAgICAgICAgICAgOiAnVG/DoG4gYuG7mSAzMCBRdeG6rW4gSHV54buHbiBIw6AgTuG7mWknXG4gICAgICAgICAgICA6IGFjdGl2ZVRhYi50b1VwcGVyQ2FzZSgpXG4gICAgICAgIH1cbiAgICAgICAgdG90YWxVbml0c0NvdW50PXt1bml0cy5sZW5ndGh9XG4gICAgICAgIGJhc2VtYXBOYW1lPVwiVuG7hyB0aW5oIEVzcmkgLyBC4bqjbiDEkeG7kyBz4buRIFZOLTIwMDBcIlxuICAgICAgLz5cblxuICAgICAgey8qIEFkZCAvIEVkaXQgVW5pdCBNb2RhbCAqL31cbiAgICAgIDxVbml0TW9kYWxcbiAgICAgICAgaXNPcGVuPXtpc0FkZEVkaXRNb2RhbE9wZW59XG4gICAgICAgIG9uQ2xvc2U9eygpID0+IHtcbiAgICAgICAgICBzZXRJc0FkZEVkaXRNb2RhbE9wZW4oZmFsc2UpO1xuICAgICAgICAgIHNldEVkaXRpbmdVbml0KG51bGwpO1xuICAgICAgICB9fVxuICAgICAgICBvblNhdmU9e2hhbmRsZVNhdmVVbml0fVxuICAgICAgICBpbml0aWFsVW5pdD17ZWRpdGluZ1VuaXR9XG4gICAgICAvPlxuXG4gICAgICB7LyogUG9zdEdJUyBDb25uZWN0aW9uICYgU2NoZW1hIE1vZGFsICovfVxuICAgICAgPFBvc3RnaXNNb2RhbFxuICAgICAgICBpc09wZW49e2lzUG9zdGdpc01vZGFsT3Blbn1cbiAgICAgICAgb25DbG9zZT17KCkgPT4gc2V0SXNQb3N0Z2lzTW9kYWxPcGVuKGZhbHNlKX1cbiAgICAgICAgdW5pdHM9e3VuaXRzfVxuICAgICAgLz5cbiAgICA8L2Rpdj5cbiAgKTtcbn1cbiJdfQ==
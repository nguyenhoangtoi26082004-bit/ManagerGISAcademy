const React = __vite__cjsImport0_react; const useState = __vite__cjsImport0_react["useState"];const _jsxDEV = __vite__cjsImport4_react_jsxDevRuntime["jsxDEV"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { generatePostgisSQL, unitsToCSV } from "/src/utils/postgisExport.ts";
import { unitsToGeoJSON } from "/src/utils/gisCalculations.ts";
import { Database, Plus, Download, Upload, FileCode, FileSpreadsheet, Globe, Trash2, Edit, RotateCcw, Search, CheckCircle } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/DataManagementView.tsx";
import __vite__cjsImport4_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const DataManagementView = ({ units, onOpenAddModal, onEditUnit, onDeleteUnit, onResetSeedData, onImportUnits, onDownloadZip }) => {
	const [tableSearch, setTableSearch] = useState("");
	const [downloadNotice, setDownloadNotice] = useState(null);
	const filteredUnits = units.filter((u) => u.name.toLowerCase().includes(tableSearch.toLowerCase()) || u.code.toLowerCase().includes(tableSearch.toLowerCase()) || u.district.toLowerCase().includes(tableSearch.toLowerCase()) || u.branch.toLowerCase().includes(tableSearch.toLowerCase()));
	// Download file helper
	const triggerDownload = (content, filename, type) => {
		const blob = new Blob([content], { type });
		const url = URL.createObjectURL(blob);
		const a = document.createElement("a");
		a.href = url;
		a.download = filename;
		document.body.appendChild(a);
		a.click();
		document.body.removeChild(a);
		URL.revokeObjectURL(url);
		setDownloadNotice(`Đã xuất thành công: ${filename}`);
		setTimeout(() => setDownloadNotice(null), 3e3);
	};
	const handleExportSQL = () => {
		const sql = generatePostgisSQL(units);
		triggerDownload(sql, "hanoi_military_gis_postgis.sql", "text/sql");
	};
	const handleExportGeoJSON = () => {
		const geojson = unitsToGeoJSON(units);
		triggerDownload(geojson, "hanoi_military_units_epsg4326.geojson", "application/json");
	};
	const handleExportCSV = () => {
		const csv = unitsToCSV(units);
		triggerDownload(csv, "csdl_don_vi_quan_doi_hanoi.csv", "text/csv");
	};
	// Import JSON/GeoJSON
	const handleFileUpload = (e) => {
		const file = e.target.files?.[0];
		if (!file) return;
		const reader = new FileReader();
		reader.onload = (event) => {
			try {
				const text = event.target?.result;
				const parsed = JSON.parse(text);
				if (parsed.type === "FeatureCollection" && Array.isArray(parsed.features)) {
					// Parse GeoJSON FeatureCollection
					const imported = parsed.features.map((f, idx) => {
						const props = f.properties || {};
						const [lng, lat] = f.geometry?.coordinates || [105.8, 21];
						return {
							id: props.id || `DV_IMPORT_${Date.now()}_${idx}`,
							code: props.code || `IMP-${idx + 1}`,
							name: props.name || "Đơn vị nhập khẩu GeoJSON",
							shortName: props.shortName || props.name || "ĐV nhập",
							unitType: props.unitType || "Ban Chỉ huy Quân sự",
							branch: props.branch || "Lục quân",
							parentUnit: props.parentUnit || "Bộ Quốc phòng",
							district: props.district || "Quận Ba Đình",
							ward: props.ward || "",
							address: props.address || "Hà Nội",
							latitude: lat,
							longitude: lng,
							x_vn2000: props.x_vn2000 || 58e4,
							y_vn2000: props.y_vn2000 || 232e4,
							commander: props.commander || "",
							phone: props.phone || "",
							securityLevel: props.securityLevel || "Mật",
							readinessStatus: props.readinessStatus || "Cấp 1 (Cao nhất)",
							personnelScale: props.personnelScale || "",
							mainMissions: props.mainMissions || [],
							equipmentHighlights: props.equipmentHighlights || [],
							establishedYear: props.establishedYear || 1954,
							updatedAt: new Date().toISOString()
						};
					});
					onImportUnits(imported);
					setDownloadNotice(`Đã nạp ${imported.length} đơn vị từ file GeoJSON!`);
				} else if (Array.isArray(parsed)) {
					onImportUnits(parsed);
					setDownloadNotice(`Đã nạp ${parsed.length} đơn vị từ file JSON!`);
				}
			} catch (err) {
				alert("File không hợp lệ. Vui lòng chọn file GeoJSON hoặc JSON chuẩn.");
			}
		};
		reader.readAsText(file);
	};
	return /* @__PURE__ */ _jsxDEV("div", {
		className: "flex-1 overflow-y-auto bg-[#080d19] text-white p-6 space-y-6",
		children: [
			/* @__PURE__ */ _jsxDEV("div", {
				className: "flex flex-wrap items-center justify-between gap-4 border-b border-slate-800 pb-4",
				children: [/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("h2", {
					className: "text-xl font-bold uppercase tracking-wider text-white flex items-center gap-2",
					children: [/* @__PURE__ */ _jsxDEV(Database, { className: "w-5 h-5 text-emerald-400" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 142,
						columnNumber: 13
					}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Quản trị Dữ liệu Không gian & Thuộc tính (GIS Data Management)" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 143,
						columnNumber: 13
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 141,
					columnNumber: 11
				}, this), /* @__PURE__ */ _jsxDEV("p", {
					className: "text-xs text-slate-400 mt-1",
					children: "Quản lý bảng ghi, biên tập thuộc tính không gian, xuất nhập dữ liệu PostGIS SQL, GeoJSON và CSV"
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 145,
					columnNumber: 11
				}, this)] }, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 140,
					columnNumber: 9
				}, this), /* @__PURE__ */ _jsxDEV("div", {
					className: "flex flex-wrap items-center gap-2",
					children: [/* @__PURE__ */ _jsxDEV("button", {
						onClick: onOpenAddModal,
						className: "bg-emerald-600 hover:bg-emerald-500 text-white text-xs font-semibold px-3.5 py-2 rounded-lg flex items-center gap-1.5 shadow",
						children: [/* @__PURE__ */ _jsxDEV(Plus, { className: "w-4 h-4" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 157,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Thêm đơn vị mới" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 158,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 153,
						columnNumber: 11
					}, this), /* @__PURE__ */ _jsxDEV("button", {
						onClick: onResetSeedData,
						title: "Khôi phục lại dữ liệu mẫu ban đầu của Hà Nội",
						className: "bg-slate-800 hover:bg-slate-700 text-slate-300 text-xs font-medium px-3 py-2 rounded-lg flex items-center gap-1.5 border border-slate-700",
						children: [/* @__PURE__ */ _jsxDEV(RotateCcw, { className: "w-3.5 h-3.5" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 167,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Nạp lại mẫu" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 168,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 162,
						columnNumber: 11
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 151,
					columnNumber: 9
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 139,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "bg-[#0e1628] border border-slate-800 rounded-xl p-4 shadow-md space-y-3",
				children: [
					/* @__PURE__ */ _jsxDEV("div", {
						className: "flex items-center justify-between",
						children: [/* @__PURE__ */ _jsxDEV("span", {
							className: "text-xs font-bold uppercase text-slate-300 flex items-center gap-1.5",
							children: [/* @__PURE__ */ _jsxDEV(Download, { className: "w-3.5 h-3.5 text-emerald-400" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 177,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "CÔNG CỤ XUẤT / NHẬP CƠ SỞ DỮ LIỆU GIS" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 178,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 176,
							columnNumber: 11
						}, this), downloadNotice && /* @__PURE__ */ _jsxDEV("span", {
							className: "text-xs text-emerald-300 bg-emerald-950/80 px-2.5 py-1 rounded border border-emerald-600 flex items-center gap-1",
							children: [/* @__PURE__ */ _jsxDEV(CheckCircle, { className: "w-3.5 h-3.5" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 182,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: downloadNotice }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 183,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 181,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 175,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "p-4 rounded-xl bg-gradient-to-r from-amber-950/60 via-slate-900 to-slate-900 border border-amber-600/50 flex flex-col sm:flex-row sm:items-center justify-between gap-4 shadow-lg",
						children: [/* @__PURE__ */ _jsxDEV("div", {
							className: "flex items-start gap-3",
							children: [/* @__PURE__ */ _jsxDEV("div", {
								className: "w-10 h-10 rounded-lg bg-amber-500/20 border border-amber-500/40 flex items-center justify-center text-amber-400 flex-shrink-0",
								children: /* @__PURE__ */ _jsxDEV(Download, { className: "w-5 h-5" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 192,
									columnNumber: 15
								}, this)
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 191,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("h4", {
								className: "text-sm font-bold text-white flex items-center gap-2",
								children: [/* @__PURE__ */ _jsxDEV("span", { children: "Tải Toàn Bộ Mã Nguồn Dự Án (.ZIP)" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 196,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("span", {
									className: "text-[10px] font-mono bg-amber-900/80 text-amber-200 px-2 py-0.5 rounded border border-amber-600/40",
									children: "Full Source Code + CSDL PostGIS"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 197,
									columnNumber: 17
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 195,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("p", {
								className: "text-xs text-slate-300 mt-0.5",
								children: "Đóng gói trọn bộ dự án gồm toàn bộ mã nguồn React/TypeScript, tệp cấu hình package.json, CSDL PostGIS SQL, GeoJSON và tài liệu hướng dẫn README.md chi tiết."
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 201,
								columnNumber: 15
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 194,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 190,
							columnNumber: 11
						}, this), /* @__PURE__ */ _jsxDEV("button", {
							onClick: onDownloadZip,
							className: "bg-amber-600 hover:bg-amber-500 active:scale-95 text-white font-bold text-xs px-5 py-2.5 rounded-lg flex items-center justify-center gap-2 shadow-lg transition flex-shrink-0",
							children: [/* @__PURE__ */ _jsxDEV(Download, { className: "w-4 h-4 stroke-[2.5]" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 211,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Tải Về Toàn Bộ (.zip)" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 212,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 207,
							columnNumber: 11
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 189,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3",
						children: [
							/* @__PURE__ */ _jsxDEV("button", {
								onClick: handleExportSQL,
								className: "p-3 rounded-lg bg-slate-900 border border-slate-700 hover:border-emerald-500 text-left transition group",
								children: [
									/* @__PURE__ */ _jsxDEV("div", {
										className: "flex items-center justify-between mb-1",
										children: [/* @__PURE__ */ _jsxDEV(FileCode, { className: "w-5 h-5 text-emerald-400 group-hover:scale-110 transition" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 223,
											columnNumber: 15
										}, this), /* @__PURE__ */ _jsxDEV("span", {
											className: "text-[10px] font-mono bg-emerald-950 text-emerald-300 px-1.5 py-0.5 rounded",
											children: ".SQL"
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 224,
											columnNumber: 15
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 222,
										columnNumber: 13
									}, this),
									/* @__PURE__ */ _jsxDEV("div", {
										className: "text-xs font-bold text-white group-hover:text-emerald-300",
										children: "Xuất PostGIS SQL Script"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 228,
										columnNumber: 13
									}, this),
									/* @__PURE__ */ _jsxDEV("div", {
										className: "text-[11px] text-slate-400 mt-1",
										children: "Chứa DDL, GIST spatial index & các lệnh INSERT"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 231,
										columnNumber: 13
									}, this)
								]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 218,
								columnNumber: 11
							}, this),
							/* @__PURE__ */ _jsxDEV("button", {
								onClick: handleExportGeoJSON,
								className: "p-3 rounded-lg bg-slate-900 border border-slate-700 hover:border-sky-500 text-left transition group",
								children: [
									/* @__PURE__ */ _jsxDEV("div", {
										className: "flex items-center justify-between mb-1",
										children: [/* @__PURE__ */ _jsxDEV(Globe, { className: "w-5 h-5 text-sky-400 group-hover:scale-110 transition" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 242,
											columnNumber: 15
										}, this), /* @__PURE__ */ _jsxDEV("span", {
											className: "text-[10px] font-mono bg-sky-950 text-sky-300 px-1.5 py-0.5 rounded",
											children: ".GEOJSON"
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 243,
											columnNumber: 15
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 241,
										columnNumber: 13
									}, this),
									/* @__PURE__ */ _jsxDEV("div", {
										className: "text-xs font-bold text-white group-hover:text-sky-300",
										children: "Xuất Lớp GeoJSON"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 247,
										columnNumber: 13
									}, this),
									/* @__PURE__ */ _jsxDEV("div", {
										className: "text-[11px] text-slate-400 mt-1",
										children: "Chuẩn FeatureCollection cho QGIS, ArcGIS Web"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 250,
										columnNumber: 13
									}, this)
								]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 237,
								columnNumber: 11
							}, this),
							/* @__PURE__ */ _jsxDEV("button", {
								onClick: handleExportCSV,
								className: "p-3 rounded-lg bg-slate-900 border border-slate-700 hover:border-amber-500 text-left transition group",
								children: [
									/* @__PURE__ */ _jsxDEV("div", {
										className: "flex items-center justify-between mb-1",
										children: [/* @__PURE__ */ _jsxDEV(FileSpreadsheet, { className: "w-5 h-5 text-amber-400 group-hover:scale-110 transition" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 261,
											columnNumber: 15
										}, this), /* @__PURE__ */ _jsxDEV("span", {
											className: "text-[10px] font-mono bg-amber-950 text-amber-300 px-1.5 py-0.5 rounded",
											children: ".CSV"
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 262,
											columnNumber: 15
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 260,
										columnNumber: 13
									}, this),
									/* @__PURE__ */ _jsxDEV("div", {
										className: "text-xs font-bold text-white group-hover:text-amber-300",
										children: "Xuất Bảng Excel / CSV"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 266,
										columnNumber: 13
									}, this),
									/* @__PURE__ */ _jsxDEV("div", {
										className: "text-[11px] text-slate-400 mt-1",
										children: "Bao gồm cả cột tọa độ VN2000 (m) và WGS84"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 269,
										columnNumber: 13
									}, this)
								]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 256,
								columnNumber: 11
							}, this),
							/* @__PURE__ */ _jsxDEV("label", {
								className: "p-3 rounded-lg bg-slate-900 border border-dashed border-slate-700 hover:border-purple-500 text-left transition group cursor-pointer block",
								children: [
									/* @__PURE__ */ _jsxDEV("div", {
										className: "flex items-center justify-between mb-1",
										children: [/* @__PURE__ */ _jsxDEV(Upload, { className: "w-5 h-5 text-purple-400 group-hover:scale-110 transition" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 277,
											columnNumber: 15
										}, this), /* @__PURE__ */ _jsxDEV("span", {
											className: "text-[10px] font-mono bg-purple-950 text-purple-300 px-1.5 py-0.5 rounded",
											children: "UPLOAD"
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 278,
											columnNumber: 15
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 276,
										columnNumber: 13
									}, this),
									/* @__PURE__ */ _jsxDEV("div", {
										className: "text-xs font-bold text-white group-hover:text-purple-300",
										children: "Nhập Dữ liệu GeoJSON"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 282,
										columnNumber: 13
									}, this),
									/* @__PURE__ */ _jsxDEV("div", {
										className: "text-[11px] text-slate-400 mt-1",
										children: "Tải file .geojson hoặc .json vào hệ thống"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 285,
										columnNumber: 13
									}, this),
									/* @__PURE__ */ _jsxDEV("input", {
										type: "file",
										accept: ".geojson,.json",
										onChange: handleFileUpload,
										className: "hidden"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 288,
										columnNumber: 13
									}, this)
								]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 275,
								columnNumber: 11
							}, this)
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 216,
						columnNumber: 9
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 174,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "bg-[#0e1628] border border-slate-800 rounded-xl overflow-hidden shadow-lg space-y-3 p-4",
				children: [/* @__PURE__ */ _jsxDEV("div", {
					className: "flex flex-wrap items-center justify-between gap-3",
					children: [/* @__PURE__ */ _jsxDEV("div", {
						className: "relative w-full sm:w-72",
						children: [/* @__PURE__ */ _jsxDEV("input", {
							type: "text",
							placeholder: "Tìm kiếm trong bảng...",
							value: tableSearch,
							onChange: (e) => setTableSearch(e.target.value),
							className: "w-full bg-[#090f1d] border border-slate-700 rounded-lg px-3 py-1.5 text-xs text-white placeholder-slate-500 focus:outline-none focus:border-emerald-500 pl-8"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 302,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV(Search, { className: "w-3.5 h-3.5 text-slate-400 absolute left-2.5 top-2.5" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 309,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 301,
						columnNumber: 11
					}, this), /* @__PURE__ */ _jsxDEV("div", {
						className: "text-xs text-slate-400",
						children: [
							"Hiển thị ",
							/* @__PURE__ */ _jsxDEV("strong", {
								className: "text-white",
								children: filteredUnits.length
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 312,
								columnNumber: 22
							}, this),
							" / ",
							units.length,
							" ",
							"bản ghi"
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 311,
						columnNumber: 11
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 300,
					columnNumber: 9
				}, this), /* @__PURE__ */ _jsxDEV("div", {
					className: "overflow-x-auto border border-slate-800 rounded-lg",
					children: /* @__PURE__ */ _jsxDEV("table", {
						className: "w-full text-left text-xs text-slate-300",
						children: [/* @__PURE__ */ _jsxDEV("thead", {
							className: "bg-[#090f1d] text-slate-400 uppercase text-[10px] tracking-wider font-mono border-b border-slate-800",
							children: /* @__PURE__ */ _jsxDEV("tr", { children: [
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Mã đơn vị"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 321,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Tên đơn vị quân đội"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 322,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Cấp / Loại hình"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 323,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Binh chủng"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 324,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Quận / Huyện"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 325,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3 font-mono",
									children: "Tọa độ VN-2000 (X/Y)"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 326,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "Trực ban"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 327,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3",
									children: "SSCĐ"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 328,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("th", {
									className: "p-3 text-center",
									children: "Hành động"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 329,
									columnNumber: 17
								}, this)
							] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 320,
								columnNumber: 15
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 319,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("tbody", {
							className: "divide-y divide-slate-800 font-sans",
							children: filteredUnits.map((u) => /* @__PURE__ */ _jsxDEV("tr", {
								className: "hover:bg-slate-800/60 transition",
								children: [
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 font-mono font-bold text-emerald-400 whitespace-nowrap",
										children: u.code
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 335,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 font-semibold text-white min-w-[200px]",
										children: u.name
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 338,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 text-[11px] text-slate-400 whitespace-nowrap",
										children: u.unitType
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 339,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 whitespace-nowrap",
										children: u.branch
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 340,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 whitespace-nowrap",
										children: u.district
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 341,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 font-mono text-[11px] text-sky-300 whitespace-nowrap",
										children: [
											"X: ",
											u.x_vn2000.toLocaleString(),
											"m",
											/* @__PURE__ */ _jsxDEV("br", {}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 344,
												columnNumber: 21
											}, this),
											"Y: ",
											u.y_vn2000.toLocaleString(),
											"m"
										]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 342,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 font-mono text-emerald-300 whitespace-nowrap",
										children: u.phone
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 347,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 whitespace-nowrap",
										children: /* @__PURE__ */ _jsxDEV("span", {
											className: `text-[10px] px-2 py-0.5 rounded font-medium ${u.readinessStatus.includes("Cấp 1") ? "bg-rose-950 text-rose-300" : "bg-emerald-950 text-emerald-300"}`,
											children: u.readinessStatus
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 349,
											columnNumber: 21
										}, this)
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 348,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("td", {
										className: "p-3 text-center whitespace-nowrap",
										children: /* @__PURE__ */ _jsxDEV("div", {
											className: "flex items-center justify-center gap-1.5",
											children: [/* @__PURE__ */ _jsxDEV("button", {
												onClick: () => onEditUnit(u),
												title: "Sửa bản ghi",
												className: "p-1.5 rounded bg-slate-800 hover:bg-emerald-600 text-slate-300 hover:text-white transition",
												children: /* @__PURE__ */ _jsxDEV(Edit, { className: "w-3.5 h-3.5" }, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 366,
													columnNumber: 25
												}, this)
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 361,
												columnNumber: 23
											}, this), /* @__PURE__ */ _jsxDEV("button", {
												onClick: () => {
													if (confirm(`Bạn có chắc muốn xóa đơn vị "${u.name}" khỏi CSDL?`)) {
														onDeleteUnit(u.id);
													}
												},
												title: "Xóa bản ghi",
												className: "p-1.5 rounded bg-slate-800 hover:bg-rose-600 text-slate-300 hover:text-white transition",
												children: /* @__PURE__ */ _jsxDEV(Trash2, { className: "w-3.5 h-3.5" }, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 377,
													columnNumber: 25
												}, this)
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 368,
												columnNumber: 23
											}, this)]
										}, void 0, true, {
											fileName: _jsxFileName,
											lineNumber: 360,
											columnNumber: 21
										}, this)
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 359,
										columnNumber: 19
									}, this)
								]
							}, u.id, true, {
								fileName: _jsxFileName,
								lineNumber: 334,
								columnNumber: 17
							}, this))
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 332,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 318,
						columnNumber: 11
					}, this)
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 317,
					columnNumber: 9
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 299,
				columnNumber: 7
			}, this)
		]
	}, void 0, true, {
		fileName: _jsxFileName,
		lineNumber: 137,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,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Q0UsZUFBZSxXQUFXLENBQUM7WUFDM0IsT0FBTTtZQUNOLFdBQVU7c0JBRVYsd0JBQUMsTUFBRCxFQUFNLFdBQVUsY0FBZTs7Ozs7V0FDekI7Ozs7cUJBQ1Isd0JBQUMsVUFBRDtZQUNFLGVBQWU7YUFDYixJQUFJLFFBQVEsZ0NBQWdDLEVBQUUsS0FBSyxhQUFhLEdBQUc7Y0FDakUsYUFBYSxFQUFFLEVBQUU7YUFDbkI7WUFDRjtZQUNBLE9BQU07WUFDTixXQUFVO3NCQUVWLHdCQUFDLFFBQUQsRUFBUSxXQUFVLGNBQWU7Ozs7O1dBQzNCOzs7O21CQUNMOzs7Ozs7U0FDSDs7Ozs7UUFDRjtVQS9DSyxFQUFFOzs7O2NBK0NQLENBQ0w7TUFDSTs7OztjQUNGOzs7Ozs7SUFDSjs7OztZQUNGOzs7Ozs7RUFDRjs7Ozs7O0FBRVQiLCJuYW1lcyI6W10sInNvdXJjZXMiOlsiRGF0YU1hbmFnZW1lbnRWaWV3LnRzeCJdLCJ2ZXJzaW9uIjozLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgUmVhY3QsIHsgdXNlU3RhdGUgfSBmcm9tICdyZWFjdCc7XG5pbXBvcnQgeyBNaWxpdGFyeVVuaXQgfSBmcm9tICcuLi90eXBlcy9naXMnO1xuaW1wb3J0IHsgZ2VuZXJhdGVQb3N0Z2lzU1FMLCB1bml0c1RvQ1NWIH0gZnJvbSAnLi4vdXRpbHMvcG9zdGdpc0V4cG9ydCc7XG5pbXBvcnQgeyB1bml0c1RvR2VvSlNPTiB9IGZyb20gJy4uL3V0aWxzL2dpc0NhbGN1bGF0aW9ucyc7XG5pbXBvcnQge1xuICBEYXRhYmFzZSxcbiAgUGx1cyxcbiAgRG93bmxvYWQsXG4gIFVwbG9hZCxcbiAgRmlsZUNvZGUsXG4gIEZpbGVTcHJlYWRzaGVldCxcbiAgR2xvYmUsXG4gIFRyYXNoMixcbiAgRWRpdCxcbiAgUm90YXRlQ2N3LFxuICBTZWFyY2gsXG4gIENoZWNrQ2lyY2xlLFxufSBmcm9tICdsdWNpZGUtcmVhY3QnO1xuXG5pbnRlcmZhY2UgRGF0YU1hbmFnZW1lbnRWaWV3UHJvcHMge1xuICB1bml0czogTWlsaXRhcnlVbml0W107XG4gIG9uT3BlbkFkZE1vZGFsOiAoKSA9PiB2b2lkO1xuICBvbkVkaXRVbml0OiAodW5pdDogTWlsaXRhcnlVbml0KSA9PiB2b2lkO1xuICBvbkRlbGV0ZVVuaXQ6IChpZDogc3RyaW5nKSA9PiB2b2lkO1xuICBvblJlc2V0U2VlZERhdGE6ICgpID0+IHZvaWQ7XG4gIG9uSW1wb3J0VW5pdHM6IChuZXdVbml0czogTWlsaXRhcnlVbml0W10pID0+IHZvaWQ7XG4gIG9uRG93bmxvYWRaaXA6ICgpID0+IHZvaWQ7XG59XG5cbmV4cG9ydCBjb25zdCBEYXRhTWFuYWdlbWVudFZpZXc6IFJlYWN0LkZDPERhdGFNYW5hZ2VtZW50Vmlld1Byb3BzPiA9ICh7XG4gIHVuaXRzLFxuICBvbk9wZW5BZGRNb2RhbCxcbiAgb25FZGl0VW5pdCxcbiAgb25EZWxldGVVbml0LFxuICBvblJlc2V0U2VlZERhdGEsXG4gIG9uSW1wb3J0VW5pdHMsXG4gIG9uRG93bmxvYWRaaXAsXG59KSA9PiB7XG4gIGNvbnN0IFt0YWJsZVNlYXJjaCwgc2V0VGFibGVTZWFyY2hdID0gdXNlU3RhdGUoJycpO1xuICBjb25zdCBbZG93bmxvYWROb3RpY2UsIHNldERvd25sb2FkTm90aWNlXSA9IHVzZVN0YXRlPHN0cmluZyB8IG51bGw+KG51bGwpO1xuXG4gIGNvbnN0IGZpbHRlcmVkVW5pdHMgPSB1bml0cy5maWx0ZXIoXG4gICAgKHUpID0+XG4gICAgICB1Lm5hbWUudG9Mb3dlckNhc2UoKS5pbmNsdWRlcyh0YWJsZVNlYXJjaC50b0xvd2VyQ2FzZSgpKSB8fFxuICAgICAgdS5jb2RlLnRvTG93ZXJDYXNlKCkuaW5jbHVkZXModGFibGVTZWFyY2gudG9Mb3dlckNhc2UoKSkgfHxcbiAgICAgIHUuZGlzdHJpY3QudG9Mb3dlckNhc2UoKS5pbmNsdWRlcyh0YWJsZVNlYXJjaC50b0xvd2VyQ2FzZSgpKSB8fFxuICAgICAgdS5icmFuY2gudG9Mb3dlckNhc2UoKS5pbmNsdWRlcyh0YWJsZVNlYXJjaC50b0xvd2VyQ2FzZSgpKVxuICApO1xuXG4gIC8vIERvd25sb2FkIGZpbGUgaGVscGVyXG4gIGNvbnN0IHRyaWdnZXJEb3dubG9hZCA9IChjb250ZW50OiBzdHJpbmcsIGZpbGVuYW1lOiBzdHJpbmcsIHR5cGU6IHN0cmluZykgPT4ge1xuICAgIGNvbnN0IGJsb2IgPSBuZXcgQmxvYihbY29udGVudF0sIHsgdHlwZSB9KTtcbiAgICBjb25zdCB1cmwgPSBVUkwuY3JlYXRlT2JqZWN0VVJMKGJsb2IpO1xuICAgIGNvbnN0IGEgPSBkb2N1bWVudC5jcmVhdGVFbGVtZW50KCdhJyk7XG4gICAgYS5ocmVmID0gdXJsO1xuICAgIGEuZG93bmxvYWQgPSBmaWxlbmFtZTtcbiAgICBkb2N1bWVudC5ib2R5LmFwcGVuZENoaWxkKGEpO1xuICAgIGEuY2xpY2soKTtcbiAgICBkb2N1bWVudC5ib2R5LnJlbW92ZUNoaWxkKGEpO1xuICAgIFVSTC5yZXZva2VPYmplY3RVUkwodXJsKTtcblxuICAgIHNldERvd25sb2FkTm90aWNlKGDEkMOjIHh14bqldCB0aMOgbmggY8O0bmc6ICR7ZmlsZW5hbWV9YCk7XG4gICAgc2V0VGltZW91dCgoKSA9PiBzZXREb3dubG9hZE5vdGljZShudWxsKSwgMzAwMCk7XG4gIH07XG5cbiAgY29uc3QgaGFuZGxlRXhwb3J0U1FMID0gKCkgPT4ge1xuICAgIGNvbnN0IHNxbCA9IGdlbmVyYXRlUG9zdGdpc1NRTCh1bml0cyk7XG4gICAgdHJpZ2dlckRvd25sb2FkKHNxbCwgJ2hhbm9pX21pbGl0YXJ5X2dpc19wb3N0Z2lzLnNxbCcsICd0ZXh0L3NxbCcpO1xuICB9O1xuXG4gIGNvbnN0IGhhbmRsZUV4cG9ydEdlb0pTT04gPSAoKSA9PiB7XG4gICAgY29uc3QgZ2VvanNvbiA9IHVuaXRzVG9HZW9KU09OKHVuaXRzKTtcbiAgICB0cmlnZ2VyRG93bmxvYWQoZ2VvanNvbiwgJ2hhbm9pX21pbGl0YXJ5X3VuaXRzX2Vwc2c0MzI2Lmdlb2pzb24nLCAnYXBwbGljYXRpb24vanNvbicpO1xuICB9O1xuXG4gIGNvbnN0IGhhbmRsZUV4cG9ydENTViA9ICgpID0+IHtcbiAgICBjb25zdCBjc3YgPSB1bml0c1RvQ1NWKHVuaXRzKTtcbiAgICB0cmlnZ2VyRG93bmxvYWQoY3N2LCAnY3NkbF9kb25fdmlfcXVhbl9kb2lfaGFub2kuY3N2JywgJ3RleHQvY3N2Jyk7XG4gIH07XG5cbiAgLy8gSW1wb3J0IEpTT04vR2VvSlNPTlxuICBjb25zdCBoYW5kbGVGaWxlVXBsb2FkID0gKGU6IFJlYWN0LkNoYW5nZUV2ZW50PEhUTUxJbnB1dEVsZW1lbnQ+KSA9PiB7XG4gICAgY29uc3QgZmlsZSA9IGUudGFyZ2V0LmZpbGVzPy5bMF07XG4gICAgaWYgKCFmaWxlKSByZXR1cm47XG5cbiAgICBjb25zdCByZWFkZXIgPSBuZXcgRmlsZVJlYWRlcigpO1xuICAgIHJlYWRlci5vbmxvYWQgPSAoZXZlbnQpID0+IHtcbiAgICAgIHRyeSB7XG4gICAgICAgIGNvbnN0IHRleHQgPSBldmVudC50YXJnZXQ/LnJlc3VsdCBhcyBzdHJpbmc7XG4gICAgICAgIGNvbnN0IHBhcnNlZCA9IEpTT04ucGFyc2UodGV4dCk7XG5cbiAgICAgICAgaWYgKHBhcnNlZC50eXBlID09PSAnRmVhdHVyZUNvbGxlY3Rpb24nICYmIEFycmF5LmlzQXJyYXkocGFyc2VkLmZlYXR1cmVzKSkge1xuICAgICAgICAgIC8vIFBhcnNlIEdlb0pTT04gRmVhdHVyZUNvbGxlY3Rpb25cbiAgICAgICAgICBjb25zdCBpbXBvcnRlZDogTWlsaXRhcnlVbml0W10gPSBwYXJzZWQuZmVhdHVyZXMubWFwKChmOiBhbnksIGlkeDogbnVtYmVyKSA9PiB7XG4gICAgICAgICAgICBjb25zdCBwcm9wcyA9IGYucHJvcGVydGllcyB8fCB7fTtcbiAgICAgICAgICAgIGNvbnN0IFtsbmcsIGxhdF0gPSBmLmdlb21ldHJ5Py5jb29yZGluYXRlcyB8fCBbMTA1LjgsIDIxLjBdO1xuICAgICAgICAgICAgcmV0dXJuIHtcbiAgICAgICAgICAgICAgaWQ6IHByb3BzLmlkIHx8IGBEVl9JTVBPUlRfJHtEYXRlLm5vdygpfV8ke2lkeH1gLFxuICAgICAgICAgICAgICBjb2RlOiBwcm9wcy5jb2RlIHx8IGBJTVAtJHtpZHggKyAxfWAsXG4gICAgICAgICAgICAgIG5hbWU6IHByb3BzLm5hbWUgfHwgJ8SQxqFuIHbhu4sgbmjhuq1wIGto4bqpdSBHZW9KU09OJyxcbiAgICAgICAgICAgICAgc2hvcnROYW1lOiBwcm9wcy5zaG9ydE5hbWUgfHwgcHJvcHMubmFtZSB8fCAnxJBWIG5o4bqtcCcsXG4gICAgICAgICAgICAgIHVuaXRUeXBlOiBwcm9wcy51bml0VHlwZSB8fCAnQmFuIENo4buJIGh1eSBRdcOibiBz4buxJyxcbiAgICAgICAgICAgICAgYnJhbmNoOiBwcm9wcy5icmFuY2ggfHwgJ0zhu6VjIHF1w6JuJyxcbiAgICAgICAgICAgICAgcGFyZW50VW5pdDogcHJvcHMucGFyZW50VW5pdCB8fCAnQuG7mSBRdeG7kWMgcGjDsm5nJyxcbiAgICAgICAgICAgICAgZGlzdHJpY3Q6IHByb3BzLmRpc3RyaWN0IHx8ICdRdeG6rW4gQmEgxJDDrG5oJyxcbiAgICAgICAgICAgICAgd2FyZDogcHJvcHMud2FyZCB8fCAnJyxcbiAgICAgICAgICAgICAgYWRkcmVzczogcHJvcHMuYWRkcmVzcyB8fCAnSMOgIE7hu5lpJyxcbiAgICAgICAgICAgICAgbGF0aXR1ZGU6IGxhdCxcbiAgICAgICAgICAgICAgbG9uZ2l0dWRlOiBsbmcsXG4gICAgICAgICAgICAgIHhfdm4yMDAwOiBwcm9wcy54X3ZuMjAwMCB8fCA1ODAwMDAsXG4gICAgICAgICAgICAgIHlfdm4yMDAwOiBwcm9wcy55X3ZuMjAwMCB8fCAyMzIwMDAwLFxuICAgICAgICAgICAgICBjb21tYW5kZXI6IHByb3BzLmNvbW1hbmRlciB8fCAnJyxcbiAgICAgICAgICAgICAgcGhvbmU6IHByb3BzLnBob25lIHx8ICcnLFxuICAgICAgICAgICAgICBzZWN1cml0eUxldmVsOiBwcm9wcy5zZWN1cml0eUxldmVsIHx8ICdN4bqtdCcsXG4gICAgICAgICAgICAgIHJlYWRpbmVzc1N0YXR1czogcHJvcHMucmVhZGluZXNzU3RhdHVzIHx8ICdD4bqlcCAxIChDYW8gbmjhuqV0KScsXG4gICAgICAgICAgICAgIHBlcnNvbm5lbFNjYWxlOiBwcm9wcy5wZXJzb25uZWxTY2FsZSB8fCAnJyxcbiAgICAgICAgICAgICAgbWFpbk1pc3Npb25zOiBwcm9wcy5tYWluTWlzc2lvbnMgfHwgW10sXG4gICAgICAgICAgICAgIGVxdWlwbWVudEhpZ2hsaWdodHM6IHByb3BzLmVxdWlwbWVudEhpZ2hsaWdodHMgfHwgW10sXG4gICAgICAgICAgICAgIGVzdGFibGlzaGVkWWVhcjogcHJvcHMuZXN0YWJsaXNoZWRZZWFyIHx8IDE5NTQsXG4gICAgICAgICAgICAgIHVwZGF0ZWRBdDogbmV3IERhdGUoKS50b0lTT1N0cmluZygpLFxuICAgICAgICAgICAgfTtcbiAgICAgICAgICB9KTtcbiAgICAgICAgICBvbkltcG9ydFVuaXRzKGltcG9ydGVkKTtcbiAgICAgICAgICBzZXREb3dubG9hZE5vdGljZShgxJDDoyBu4bqhcCAke2ltcG9ydGVkLmxlbmd0aH0gxJHGoW4gduG7iyB04burIGZpbGUgR2VvSlNPTiFgKTtcbiAgICAgICAgfSBlbHNlIGlmIChBcnJheS5pc0FycmF5KHBhcnNlZCkpIHtcbiAgICAgICAgICBvbkltcG9ydFVuaXRzKHBhcnNlZCk7XG4gICAgICAgICAgc2V0RG93bmxvYWROb3RpY2UoYMSQw6MgbuG6oXAgJHtwYXJzZWQubGVuZ3RofSDEkcahbiB24buLIHThu6sgZmlsZSBKU09OIWApO1xuICAgICAgICB9XG4gICAgICB9IGNhdGNoIChlcnIpIHtcbiAgICAgICAgYWxlcnQoJ0ZpbGUga2jDtG5nIGjhu6NwIGzhu4cuIFZ1aSBsw7JuZyBjaOG7jW4gZmlsZSBHZW9KU09OIGhv4bq3YyBKU09OIGNodeG6qW4uJyk7XG4gICAgICB9XG4gICAgfTtcbiAgICByZWFkZXIucmVhZEFzVGV4dChmaWxlKTtcbiAgfTtcblxuICByZXR1cm4gKFxuICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleC0xIG92ZXJmbG93LXktYXV0byBiZy1bIzA4MGQxOV0gdGV4dC13aGl0ZSBwLTYgc3BhY2UteS02XCI+XG4gICAgICB7LyogVGl0bGUgJiBBY3Rpb25zICovfVxuICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGZsZXgtd3JhcCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuIGdhcC00IGJvcmRlci1iIGJvcmRlci1zbGF0ZS04MDAgcGItNFwiPlxuICAgICAgICA8ZGl2PlxuICAgICAgICAgIDxoMiBjbGFzc05hbWU9XCJ0ZXh0LXhsIGZvbnQtYm9sZCB1cHBlcmNhc2UgdHJhY2tpbmctd2lkZXIgdGV4dC13aGl0ZSBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMlwiPlxuICAgICAgICAgICAgPERhdGFiYXNlIGNsYXNzTmFtZT1cInctNSBoLTUgdGV4dC1lbWVyYWxkLTQwMFwiIC8+XG4gICAgICAgICAgICA8c3Bhbj5RdeG6o24gdHLhu4sgROG7ryBsaeG7h3UgS2jDtG5nIGdpYW4gJiBUaHXhu5ljIHTDrW5oIChHSVMgRGF0YSBNYW5hZ2VtZW50KTwvc3Bhbj5cbiAgICAgICAgICA8L2gyPlxuICAgICAgICAgIDxwIGNsYXNzTmFtZT1cInRleHQteHMgdGV4dC1zbGF0ZS00MDAgbXQtMVwiPlxuICAgICAgICAgICAgUXXhuqNuIGzDvSBi4bqjbmcgZ2hpLCBiacOqbiB04bqtcCB0aHXhu5ljIHTDrW5oIGtow7RuZyBnaWFuLCB4deG6pXQgbmjhuq1wIGThu68gbGnhu4d1IFBvc3RHSVMgU1FMLFxuICAgICAgICAgICAgR2VvSlNPTiB2w6AgQ1NWXG4gICAgICAgICAgPC9wPlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggZmxleC13cmFwIGl0ZW1zLWNlbnRlciBnYXAtMlwiPlxuICAgICAgICAgIHsvKiBBZGQgdW5pdCAqL31cbiAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICBvbkNsaWNrPXtvbk9wZW5BZGRNb2RhbH1cbiAgICAgICAgICAgIGNsYXNzTmFtZT1cImJnLWVtZXJhbGQtNjAwIGhvdmVyOmJnLWVtZXJhbGQtNTAwIHRleHQtd2hpdGUgdGV4dC14cyBmb250LXNlbWlib2xkIHB4LTMuNSBweS0yIHJvdW5kZWQtbGcgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTEuNSBzaGFkb3dcIlxuICAgICAgICAgID5cbiAgICAgICAgICAgIDxQbHVzIGNsYXNzTmFtZT1cInctNCBoLTRcIiAvPlxuICAgICAgICAgICAgPHNwYW4+VGjDqm0gxJHGoW4gduG7iyBt4bubaTwvc3Bhbj5cbiAgICAgICAgICA8L2J1dHRvbj5cblxuICAgICAgICAgIHsvKiBSZXNldCBzZWVkICovfVxuICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgIG9uQ2xpY2s9e29uUmVzZXRTZWVkRGF0YX1cbiAgICAgICAgICAgIHRpdGxlPVwiS2jDtGkgcGjhu6VjIGzhuqFpIGThu68gbGnhu4d1IG3huqt1IGJhbiDEkeG6p3UgY+G7p2EgSMOgIE7hu5lpXCJcbiAgICAgICAgICAgIGNsYXNzTmFtZT1cImJnLXNsYXRlLTgwMCBob3ZlcjpiZy1zbGF0ZS03MDAgdGV4dC1zbGF0ZS0zMDAgdGV4dC14cyBmb250LW1lZGl1bSBweC0zIHB5LTIgcm91bmRlZC1sZyBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41IGJvcmRlciBib3JkZXItc2xhdGUtNzAwXCJcbiAgICAgICAgICA+XG4gICAgICAgICAgICA8Um90YXRlQ2N3IGNsYXNzTmFtZT1cInctMy41IGgtMy41XCIgLz5cbiAgICAgICAgICAgIDxzcGFuPk7huqFwIGzhuqFpIG3huqt1PC9zcGFuPlxuICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICA8L2Rpdj5cbiAgICAgIDwvZGl2PlxuXG4gICAgICB7LyogRXhwb3J0ICYgSW1wb3J0IFRvb2xib3ggKi99XG4gICAgICA8ZGl2IGNsYXNzTmFtZT1cImJnLVsjMGUxNjI4XSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCByb3VuZGVkLXhsIHAtNCBzaGFkb3ctbWQgc3BhY2UteS0zXCI+XG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuXCI+XG4gICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC14cyBmb250LWJvbGQgdXBwZXJjYXNlIHRleHQtc2xhdGUtMzAwIGZsZXggaXRlbXMtY2VudGVyIGdhcC0xLjVcIj5cbiAgICAgICAgICAgIDxEb3dubG9hZCBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNSB0ZXh0LWVtZXJhbGQtNDAwXCIgLz5cbiAgICAgICAgICAgIDxzcGFuPkPDlE5HIEPhu6QgWFXhuqRUIC8gTkjhuqxQIEPGoCBT4bueIEThu64gTEnhu4ZVIEdJUzwvc3Bhbj5cbiAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAge2Rvd25sb2FkTm90aWNlICYmIChcbiAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQteHMgdGV4dC1lbWVyYWxkLTMwMCBiZy1lbWVyYWxkLTk1MC84MCBweC0yLjUgcHktMSByb3VuZGVkIGJvcmRlciBib3JkZXItZW1lcmFsZC02MDAgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTFcIj5cbiAgICAgICAgICAgICAgPENoZWNrQ2lyY2xlIGNsYXNzTmFtZT1cInctMy41IGgtMy41XCIgLz5cbiAgICAgICAgICAgICAgPHNwYW4+e2Rvd25sb2FkTm90aWNlfTwvc3Bhbj5cbiAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICApfVxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICB7LyogSEVSTyBET1dOTE9BRCBQUk9KRUNUIFpJUCBCVVRUT04gKi99XG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwicC00IHJvdW5kZWQteGwgYmctZ3JhZGllbnQtdG8tciBmcm9tLWFtYmVyLTk1MC82MCB2aWEtc2xhdGUtOTAwIHRvLXNsYXRlLTkwMCBib3JkZXIgYm9yZGVyLWFtYmVyLTYwMC81MCBmbGV4IGZsZXgtY29sIHNtOmZsZXgtcm93IHNtOml0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gZ2FwLTQgc2hhZG93LWxnXCI+XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLXN0YXJ0IGdhcC0zXCI+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInctMTAgaC0xMCByb3VuZGVkLWxnIGJnLWFtYmVyLTUwMC8yMCBib3JkZXIgYm9yZGVyLWFtYmVyLTUwMC80MCBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWNlbnRlciB0ZXh0LWFtYmVyLTQwMCBmbGV4LXNocmluay0wXCI+XG4gICAgICAgICAgICAgIDxEb3dubG9hZCBjbGFzc05hbWU9XCJ3LTUgaC01XCIgLz5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgPGg0IGNsYXNzTmFtZT1cInRleHQtc20gZm9udC1ib2xkIHRleHQtd2hpdGUgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTJcIj5cbiAgICAgICAgICAgICAgICA8c3Bhbj5U4bqjaSBUb8OgbiBC4buZIE3DoyBOZ3Xhu5NuIEThu7Egw4FuICguWklQKTwvc3Bhbj5cbiAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSBmb250LW1vbm8gYmctYW1iZXItOTAwLzgwIHRleHQtYW1iZXItMjAwIHB4LTIgcHktMC41IHJvdW5kZWQgYm9yZGVyIGJvcmRlci1hbWJlci02MDAvNDBcIj5cbiAgICAgICAgICAgICAgICAgIEZ1bGwgU291cmNlIENvZGUgKyBDU0RMIFBvc3RHSVNcbiAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgIDwvaDQ+XG4gICAgICAgICAgICAgIDxwIGNsYXNzTmFtZT1cInRleHQteHMgdGV4dC1zbGF0ZS0zMDAgbXQtMC41XCI+XG4gICAgICAgICAgICAgICAgxJDDs25nIGfDs2kgdHLhu41uIGLhu5kgZOG7sSDDoW4gZ+G7k20gdG/DoG4gYuG7mSBtw6Mgbmd14buTbiBSZWFjdC9UeXBlU2NyaXB0LCB04buHcCBj4bqldSBow6xuaCBwYWNrYWdlLmpzb24sXG4gICAgICAgICAgICAgICAgQ1NETCBQb3N0R0lTIFNRTCwgR2VvSlNPTiB2w6AgdMOgaSBsaeG7h3UgaMaw4bubbmcgZOG6q24gUkVBRE1FLm1kIGNoaSB0aeG6v3QuXG4gICAgICAgICAgICAgIDwvcD5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgIG9uQ2xpY2s9e29uRG93bmxvYWRaaXB9XG4gICAgICAgICAgICBjbGFzc05hbWU9XCJiZy1hbWJlci02MDAgaG92ZXI6YmctYW1iZXItNTAwIGFjdGl2ZTpzY2FsZS05NSB0ZXh0LXdoaXRlIGZvbnQtYm9sZCB0ZXh0LXhzIHB4LTUgcHktMi41IHJvdW5kZWQtbGcgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1jZW50ZXIgZ2FwLTIgc2hhZG93LWxnIHRyYW5zaXRpb24gZmxleC1zaHJpbmstMFwiXG4gICAgICAgICAgPlxuICAgICAgICAgICAgPERvd25sb2FkIGNsYXNzTmFtZT1cInctNCBoLTQgc3Ryb2tlLVsyLjVdXCIgLz5cbiAgICAgICAgICAgIDxzcGFuPlThuqNpIFbhu4EgVG/DoG4gQuG7mSAoLnppcCk8L3NwYW4+XG4gICAgICAgICAgPC9idXR0b24+XG4gICAgICAgIDwvZGl2PlxuXG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZ3JpZCBncmlkLWNvbHMtMSBzbTpncmlkLWNvbHMtMiBsZzpncmlkLWNvbHMtNCBnYXAtM1wiPlxuICAgICAgICAgIHsvKiBFeHBvcnQgUG9zdEdJUyBTUUwgKi99XG4gICAgICAgICAgPGJ1dHRvblxuICAgICAgICAgICAgb25DbGljaz17aGFuZGxlRXhwb3J0U1FMfVxuICAgICAgICAgICAgY2xhc3NOYW1lPVwicC0zIHJvdW5kZWQtbGcgYmctc2xhdGUtOTAwIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIGhvdmVyOmJvcmRlci1lbWVyYWxkLTUwMCB0ZXh0LWxlZnQgdHJhbnNpdGlvbiBncm91cFwiXG4gICAgICAgICAgPlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gbWItMVwiPlxuICAgICAgICAgICAgICA8RmlsZUNvZGUgY2xhc3NOYW1lPVwidy01IGgtNSB0ZXh0LWVtZXJhbGQtNDAwIGdyb3VwLWhvdmVyOnNjYWxlLTExMCB0cmFuc2l0aW9uXCIgLz5cbiAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gZm9udC1tb25vIGJnLWVtZXJhbGQtOTUwIHRleHQtZW1lcmFsZC0zMDAgcHgtMS41IHB5LTAuNSByb3VuZGVkXCI+XG4gICAgICAgICAgICAgICAgLlNRTFxuICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC14cyBmb250LWJvbGQgdGV4dC13aGl0ZSBncm91cC1ob3Zlcjp0ZXh0LWVtZXJhbGQtMzAwXCI+XG4gICAgICAgICAgICAgIFh14bqldCBQb3N0R0lTIFNRTCBTY3JpcHRcbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LVsxMXB4XSB0ZXh0LXNsYXRlLTQwMCBtdC0xXCI+XG4gICAgICAgICAgICAgIENo4bupYSBEREwsIEdJU1Qgc3BhdGlhbCBpbmRleCAmIGPDoWMgbOG7h25oIElOU0VSVFxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgPC9idXR0b24+XG5cbiAgICAgICAgICB7LyogRXhwb3J0IEdlb0pTT04gKi99XG4gICAgICAgICAgPGJ1dHRvblxuICAgICAgICAgICAgb25DbGljaz17aGFuZGxlRXhwb3J0R2VvSlNPTn1cbiAgICAgICAgICAgIGNsYXNzTmFtZT1cInAtMyByb3VuZGVkLWxnIGJnLXNsYXRlLTkwMCBib3JkZXIgYm9yZGVyLXNsYXRlLTcwMCBob3Zlcjpib3JkZXItc2t5LTUwMCB0ZXh0LWxlZnQgdHJhbnNpdGlvbiBncm91cFwiXG4gICAgICAgICAgPlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gbWItMVwiPlxuICAgICAgICAgICAgICA8R2xvYmUgY2xhc3NOYW1lPVwidy01IGgtNSB0ZXh0LXNreS00MDAgZ3JvdXAtaG92ZXI6c2NhbGUtMTEwIHRyYW5zaXRpb25cIiAvPlxuICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSBmb250LW1vbm8gYmctc2t5LTk1MCB0ZXh0LXNreS0zMDAgcHgtMS41IHB5LTAuNSByb3VuZGVkXCI+XG4gICAgICAgICAgICAgICAgLkdFT0pTT05cbiAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInRleHQteHMgZm9udC1ib2xkIHRleHQtd2hpdGUgZ3JvdXAtaG92ZXI6dGV4dC1za3ktMzAwXCI+XG4gICAgICAgICAgICAgIFh14bqldCBM4bubcCBHZW9KU09OXG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gdGV4dC1zbGF0ZS00MDAgbXQtMVwiPlxuICAgICAgICAgICAgICBDaHXhuqluIEZlYXR1cmVDb2xsZWN0aW9uIGNobyBRR0lTLCBBcmNHSVMgV2ViXG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8L2J1dHRvbj5cblxuICAgICAgICAgIHsvKiBFeHBvcnQgQ1NWICovfVxuICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgIG9uQ2xpY2s9e2hhbmRsZUV4cG9ydENTVn1cbiAgICAgICAgICAgIGNsYXNzTmFtZT1cInAtMyByb3VuZGVkLWxnIGJnLXNsYXRlLTkwMCBib3JkZXIgYm9yZGVyLXNsYXRlLTcwMCBob3Zlcjpib3JkZXItYW1iZXItNTAwIHRleHQtbGVmdCB0cmFuc2l0aW9uIGdyb3VwXCJcbiAgICAgICAgICA+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlbiBtYi0xXCI+XG4gICAgICAgICAgICAgIDxGaWxlU3ByZWFkc2hlZXQgY2xhc3NOYW1lPVwidy01IGgtNSB0ZXh0LWFtYmVyLTQwMCBncm91cC1ob3ZlcjpzY2FsZS0xMTAgdHJhbnNpdGlvblwiIC8+XG4gICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIGZvbnQtbW9ubyBiZy1hbWJlci05NTAgdGV4dC1hbWJlci0zMDAgcHgtMS41IHB5LTAuNSByb3VuZGVkXCI+XG4gICAgICAgICAgICAgICAgLkNTVlxuICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC14cyBmb250LWJvbGQgdGV4dC13aGl0ZSBncm91cC1ob3Zlcjp0ZXh0LWFtYmVyLTMwMFwiPlxuICAgICAgICAgICAgICBYdeG6pXQgQuG6o25nIEV4Y2VsIC8gQ1NWXG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gdGV4dC1zbGF0ZS00MDAgbXQtMVwiPlxuICAgICAgICAgICAgICBCYW8gZ+G7k20gY+G6oyBj4buZdCB04buNYSDEkeG7mSBWTjIwMDAgKG0pIHbDoCBXR1M4NFxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgPC9idXR0b24+XG5cbiAgICAgICAgICB7LyogSW1wb3J0IEdlb0pTT04gLyBKU09OICovfVxuICAgICAgICAgIDxsYWJlbCBjbGFzc05hbWU9XCJwLTMgcm91bmRlZC1sZyBiZy1zbGF0ZS05MDAgYm9yZGVyIGJvcmRlci1kYXNoZWQgYm9yZGVyLXNsYXRlLTcwMCBob3Zlcjpib3JkZXItcHVycGxlLTUwMCB0ZXh0LWxlZnQgdHJhbnNpdGlvbiBncm91cCBjdXJzb3ItcG9pbnRlciBibG9ja1wiPlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gbWItMVwiPlxuICAgICAgICAgICAgICA8VXBsb2FkIGNsYXNzTmFtZT1cInctNSBoLTUgdGV4dC1wdXJwbGUtNDAwIGdyb3VwLWhvdmVyOnNjYWxlLTExMCB0cmFuc2l0aW9uXCIgLz5cbiAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gZm9udC1tb25vIGJnLXB1cnBsZS05NTAgdGV4dC1wdXJwbGUtMzAwIHB4LTEuNSBweS0wLjUgcm91bmRlZFwiPlxuICAgICAgICAgICAgICAgIFVQTE9BRFxuICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC14cyBmb250LWJvbGQgdGV4dC13aGl0ZSBncm91cC1ob3Zlcjp0ZXh0LXB1cnBsZS0zMDBcIj5cbiAgICAgICAgICAgICAgTmjhuq1wIEThu68gbGnhu4d1IEdlb0pTT05cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LVsxMXB4XSB0ZXh0LXNsYXRlLTQwMCBtdC0xXCI+XG4gICAgICAgICAgICAgIFThuqNpIGZpbGUgLmdlb2pzb24gaG/hurdjIC5qc29uIHbDoG8gaOG7hyB0aOG7kW5nXG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxpbnB1dFxuICAgICAgICAgICAgICB0eXBlPVwiZmlsZVwiXG4gICAgICAgICAgICAgIGFjY2VwdD1cIi5nZW9qc29uLC5qc29uXCJcbiAgICAgICAgICAgICAgb25DaGFuZ2U9e2hhbmRsZUZpbGVVcGxvYWR9XG4gICAgICAgICAgICAgIGNsYXNzTmFtZT1cImhpZGRlblwiXG4gICAgICAgICAgICAvPlxuICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgIDwvZGl2PlxuICAgICAgPC9kaXY+XG5cbiAgICAgIHsvKiBUYWJsZSBEYXRhIFZpZXcgKi99XG4gICAgICA8ZGl2IGNsYXNzTmFtZT1cImJnLVsjMGUxNjI4XSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCByb3VuZGVkLXhsIG92ZXJmbG93LWhpZGRlbiBzaGFkb3ctbGcgc3BhY2UteS0zIHAtNFwiPlxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggZmxleC13cmFwIGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gZ2FwLTNcIj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInJlbGF0aXZlIHctZnVsbCBzbTp3LTcyXCI+XG4gICAgICAgICAgICA8aW5wdXRcbiAgICAgICAgICAgICAgdHlwZT1cInRleHRcIlxuICAgICAgICAgICAgICBwbGFjZWhvbGRlcj1cIlTDrG0ga2nhur9tIHRyb25nIGLhuqNuZy4uLlwiXG4gICAgICAgICAgICAgIHZhbHVlPXt0YWJsZVNlYXJjaH1cbiAgICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PiBzZXRUYWJsZVNlYXJjaChlLnRhcmdldC52YWx1ZSl9XG4gICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1bIzA5MGYxZF0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZC1sZyBweC0zIHB5LTEuNSB0ZXh0LXhzIHRleHQtd2hpdGUgcGxhY2Vob2xkZXItc2xhdGUtNTAwIGZvY3VzOm91dGxpbmUtbm9uZSBmb2N1czpib3JkZXItZW1lcmFsZC01MDAgcGwtOFwiXG4gICAgICAgICAgICAvPlxuICAgICAgICAgICAgPFNlYXJjaCBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNSB0ZXh0LXNsYXRlLTQwMCBhYnNvbHV0ZSBsZWZ0LTIuNSB0b3AtMi41XCIgLz5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInRleHQteHMgdGV4dC1zbGF0ZS00MDBcIj5cbiAgICAgICAgICAgIEhp4buDbiB0aOG7iyA8c3Ryb25nIGNsYXNzTmFtZT1cInRleHQtd2hpdGVcIj57ZmlsdGVyZWRVbml0cy5sZW5ndGh9PC9zdHJvbmc+IC8ge3VuaXRzLmxlbmd0aH17JyAnfVxuICAgICAgICAgICAgYuG6o24gZ2hpXG4gICAgICAgICAgPC9kaXY+XG4gICAgICAgIDwvZGl2PlxuXG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwib3ZlcmZsb3cteC1hdXRvIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHJvdW5kZWQtbGdcIj5cbiAgICAgICAgICA8dGFibGUgY2xhc3NOYW1lPVwidy1mdWxsIHRleHQtbGVmdCB0ZXh0LXhzIHRleHQtc2xhdGUtMzAwXCI+XG4gICAgICAgICAgICA8dGhlYWQgY2xhc3NOYW1lPVwiYmctWyMwOTBmMWRdIHRleHQtc2xhdGUtNDAwIHVwcGVyY2FzZSB0ZXh0LVsxMHB4XSB0cmFja2luZy13aWRlciBmb250LW1vbm8gYm9yZGVyLWIgYm9yZGVyLXNsYXRlLTgwMFwiPlxuICAgICAgICAgICAgICA8dHI+XG4gICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtM1wiPk3DoyDEkcahbiB24buLPC90aD5cbiAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0zXCI+VMOqbiDEkcahbiB24buLIHF1w6JuIMSR4buZaTwvdGg+XG4gICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtM1wiPkPhuqVwIC8gTG/huqFpIGjDrG5oPC90aD5cbiAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0zXCI+QmluaCBjaOG7p25nPC90aD5cbiAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0zXCI+UXXhuq1uIC8gSHV54buHbjwvdGg+XG4gICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtMyBmb250LW1vbm9cIj5U4buNYSDEkeG7mSBWTi0yMDAwIChYL1kpPC90aD5cbiAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0zXCI+VHLhu7FjIGJhbjwvdGg+XG4gICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtM1wiPlNTQ8SQPC90aD5cbiAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0zIHRleHQtY2VudGVyXCI+SMOgbmggxJHhu5luZzwvdGg+XG4gICAgICAgICAgICAgIDwvdHI+XG4gICAgICAgICAgICA8L3RoZWFkPlxuICAgICAgICAgICAgPHRib2R5IGNsYXNzTmFtZT1cImRpdmlkZS15IGRpdmlkZS1zbGF0ZS04MDAgZm9udC1zYW5zXCI+XG4gICAgICAgICAgICAgIHtmaWx0ZXJlZFVuaXRzLm1hcCgodSkgPT4gKFxuICAgICAgICAgICAgICAgIDx0ciBrZXk9e3UuaWR9IGNsYXNzTmFtZT1cImhvdmVyOmJnLXNsYXRlLTgwMC82MCB0cmFuc2l0aW9uXCI+XG4gICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0zIGZvbnQtbW9ubyBmb250LWJvbGQgdGV4dC1lbWVyYWxkLTQwMCB3aGl0ZXNwYWNlLW5vd3JhcFwiPlxuICAgICAgICAgICAgICAgICAgICB7dS5jb2RlfVxuICAgICAgICAgICAgICAgICAgPC90ZD5cbiAgICAgICAgICAgICAgICAgIDx0ZCBjbGFzc05hbWU9XCJwLTMgZm9udC1zZW1pYm9sZCB0ZXh0LXdoaXRlIG1pbi13LVsyMDBweF1cIj57dS5uYW1lfTwvdGQ+XG4gICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0zIHRleHQtWzExcHhdIHRleHQtc2xhdGUtNDAwIHdoaXRlc3BhY2Utbm93cmFwXCI+e3UudW5pdFR5cGV9PC90ZD5cbiAgICAgICAgICAgICAgICAgIDx0ZCBjbGFzc05hbWU9XCJwLTMgd2hpdGVzcGFjZS1ub3dyYXBcIj57dS5icmFuY2h9PC90ZD5cbiAgICAgICAgICAgICAgICAgIDx0ZCBjbGFzc05hbWU9XCJwLTMgd2hpdGVzcGFjZS1ub3dyYXBcIj57dS5kaXN0cmljdH08L3RkPlxuICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtMyBmb250LW1vbm8gdGV4dC1bMTFweF0gdGV4dC1za3ktMzAwIHdoaXRlc3BhY2Utbm93cmFwXCI+XG4gICAgICAgICAgICAgICAgICAgIFg6IHt1Lnhfdm4yMDAwLnRvTG9jYWxlU3RyaW5nKCl9bVxuICAgICAgICAgICAgICAgICAgICA8YnIgLz5cbiAgICAgICAgICAgICAgICAgICAgWToge3UueV92bjIwMDAudG9Mb2NhbGVTdHJpbmcoKX1tXG4gICAgICAgICAgICAgICAgICA8L3RkPlxuICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtMyBmb250LW1vbm8gdGV4dC1lbWVyYWxkLTMwMCB3aGl0ZXNwYWNlLW5vd3JhcFwiPnt1LnBob25lfTwvdGQ+XG4gICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0zIHdoaXRlc3BhY2Utbm93cmFwXCI+XG4gICAgICAgICAgICAgICAgICAgIDxzcGFuXG4gICAgICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPXtgdGV4dC1bMTBweF0gcHgtMiBweS0wLjUgcm91bmRlZCBmb250LW1lZGl1bSAke1xuICAgICAgICAgICAgICAgICAgICAgICAgdS5yZWFkaW5lc3NTdGF0dXMuaW5jbHVkZXMoJ0PhuqVwIDEnKVxuICAgICAgICAgICAgICAgICAgICAgICAgICA/ICdiZy1yb3NlLTk1MCB0ZXh0LXJvc2UtMzAwJ1xuICAgICAgICAgICAgICAgICAgICAgICAgICA6ICdiZy1lbWVyYWxkLTk1MCB0ZXh0LWVtZXJhbGQtMzAwJ1xuICAgICAgICAgICAgICAgICAgICAgIH1gfVxuICAgICAgICAgICAgICAgICAgICA+XG4gICAgICAgICAgICAgICAgICAgICAge3UucmVhZGluZXNzU3RhdHVzfVxuICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICA8L3RkPlxuICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtMyB0ZXh0LWNlbnRlciB3aGl0ZXNwYWNlLW5vd3JhcFwiPlxuICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktY2VudGVyIGdhcC0xLjVcIj5cbiAgICAgICAgICAgICAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICAgICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiBvbkVkaXRVbml0KHUpfVxuICAgICAgICAgICAgICAgICAgICAgICAgdGl0bGU9XCJT4butYSBi4bqjbiBnaGlcIlxuICAgICAgICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwicC0xLjUgcm91bmRlZCBiZy1zbGF0ZS04MDAgaG92ZXI6YmctZW1lcmFsZC02MDAgdGV4dC1zbGF0ZS0zMDAgaG92ZXI6dGV4dC13aGl0ZSB0cmFuc2l0aW9uXCJcbiAgICAgICAgICAgICAgICAgICAgICA+XG4gICAgICAgICAgICAgICAgICAgICAgICA8RWRpdCBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNVwiIC8+XG4gICAgICAgICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgICAgICAgICAgPGJ1dHRvblxuICAgICAgICAgICAgICAgICAgICAgICAgb25DbGljaz17KCkgPT4ge1xuICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoY29uZmlybShgQuG6oW4gY8OzIGNo4bqvYyBtdeG7kW4geMOzYSDEkcahbiB24buLIFwiJHt1Lm5hbWV9XCIga2jhu49pIENTREw/YCkpIHtcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBvbkRlbGV0ZVVuaXQodS5pZCk7XG4gICAgICAgICAgICAgICAgICAgICAgICAgIH1cbiAgICAgICAgICAgICAgICAgICAgICAgIH19XG4gICAgICAgICAgICAgICAgICAgICAgICB0aXRsZT1cIljDs2EgYuG6o24gZ2hpXCJcbiAgICAgICAgICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cInAtMS41IHJvdW5kZWQgYmctc2xhdGUtODAwIGhvdmVyOmJnLXJvc2UtNjAwIHRleHQtc2xhdGUtMzAwIGhvdmVyOnRleHQtd2hpdGUgdHJhbnNpdGlvblwiXG4gICAgICAgICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgICAgICAgPFRyYXNoMiBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNVwiIC8+XG4gICAgICAgICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgPC90ZD5cbiAgICAgICAgICAgICAgICA8L3RyPlxuICAgICAgICAgICAgICApKX1cbiAgICAgICAgICAgIDwvdGJvZHk+XG4gICAgICAgICAgPC90YWJsZT5cbiAgICAgICAgPC9kaXY+XG4gICAgICA8L2Rpdj5cbiAgICA8L2Rpdj5cbiAgKTtcbn07XG4iXX0=
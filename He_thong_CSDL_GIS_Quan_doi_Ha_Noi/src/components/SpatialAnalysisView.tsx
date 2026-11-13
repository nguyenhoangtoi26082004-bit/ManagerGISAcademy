const React = __vite__cjsImport0_react; const useState = __vite__cjsImport0_react["useState"]; const useMemo = __vite__cjsImport0_react["useMemo"];const _jsxDEV = __vite__cjsImport5_react_jsxDevRuntime["jsxDEV"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import { calculateDistanceKm } from "/src/utils/coordinateConversion.ts";
import { findNearestUnits } from "/src/utils/gisCalculations.ts";
import { HANOI_DISTRICTS } from "/src/data/hanoiDistricts.ts";
import { Crosshair, CircleDot, Compass, ArrowRight, Shield, Clock } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/SpatialAnalysisView.tsx";
import __vite__cjsImport5_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const SpatialAnalysisView = ({ units, onApplyBufferAndGoToMap, onSelectUnitAndGoToMap }) => {
	// Tab within spatial analysis
	const [activeAnalysis, setActiveAnalysis] = useState("buffer");
	// Buffer state
	const [selectedCenterUnitId, setSelectedCenterUnitId] = useState(units[0]?.id || "");
	const [bufferRadiusKm, setBufferRadiusKm] = useState(10);
	// KNN state
	const [knnTargetUnitId, setKnnTargetUnitId] = useState(units[0]?.id || "");
	const [knnCount, setKnnCount] = useState(5);
	const selectedCenterUnit = useMemo(() => units.find((u) => u.id === selectedCenterUnitId) || units[0], [units, selectedCenterUnitId]);
	const knnTargetUnit = useMemo(() => units.find((u) => u.id === knnTargetUnitId) || units[0], [units, knnTargetUnitId]);
	// Units within buffer calculation
	const unitsWithinBuffer = useMemo(() => {
		if (!selectedCenterUnit) return [];
		return units.map((u) => {
			const dist = calculateDistanceKm(selectedCenterUnit.latitude, selectedCenterUnit.longitude, u.latitude, u.longitude);
			return {
				unit: u,
				distanceKm: dist
			};
		}).filter((item) => item.distanceKm <= bufferRadiusKm && item.unit.id !== selectedCenterUnit.id).sort((a, b) => a.distanceKm - b.distanceKm);
	}, [
		units,
		selectedCenterUnit,
		bufferRadiusKm
	]);
	// KNN calculation
	const nearestResults = useMemo(() => {
		if (!knnTargetUnit) return [];
		const candidates = units.filter((u) => u.id !== knnTargetUnit.id);
		return findNearestUnits(knnTargetUnit.latitude, knnTargetUnit.longitude, candidates, knnCount);
	}, [
		units,
		knnTargetUnit,
		knnCount
	]);
	// Sector breakdown
	const sectorBreakdown = useMemo(() => {
		const sectors = {
			"Khu vực Trung tâm": {
				count: 0,
				units: [],
				districts: []
			},
			"Khu vực Phía Tây": {
				count: 0,
				units: [],
				districts: []
			},
			"Khu vực Phía Bắc": {
				count: 0,
				units: [],
				districts: []
			},
			"Khu vực Phía Nam": {
				count: 0,
				units: [],
				districts: []
			},
			"Khu vực Phía Đông": {
				count: 0,
				units: [],
				districts: []
			}
		};
		// Map districts to sectors
		const districtSectorMap = new Map();
		HANOI_DISTRICTS.forEach((d) => {
			districtSectorMap.set(d.name, d.defensiveSector);
			if (sectors[d.defensiveSector] && !sectors[d.defensiveSector].districts.includes(d.name)) {
				sectors[d.defensiveSector].districts.push(d.shortName);
			}
		});
		units.forEach((u) => {
			const sec = districtSectorMap.get(u.district) || "Khu vực Trung tâm";
			if (sectors[sec]) {
				sectors[sec].count++;
				sectors[sec].units.push(u);
			}
		});
		return sectors;
	}, [units]);
	return /* @__PURE__ */ _jsxDEV("div", {
		className: "flex-1 overflow-y-auto bg-[#080d19] text-white p-6 space-y-6",
		children: [
			/* @__PURE__ */ _jsxDEV("div", {
				className: "flex flex-wrap items-center justify-between gap-4 border-b border-slate-800 pb-4",
				children: [/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("h2", {
					className: "text-xl font-bold uppercase tracking-wider text-white flex items-center gap-2",
					children: [/* @__PURE__ */ _jsxDEV(Crosshair, { className: "w-5 h-5 text-emerald-400" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 110,
						columnNumber: 13
					}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Phân tích Không gian Quân sự GIS (Spatial Analysis)" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 111,
						columnNumber: 13
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 109,
					columnNumber: 11
				}, this), /* @__PURE__ */ _jsxDEV("p", {
					className: "text-xs text-slate-400 mt-1",
					children: "Ứng dụng các thuật toán GIS không gian: Vùng đệm bán kính (Buffer Zone), K-NN đơn vị gần nhất và phân bổ thế trận phòng thủ"
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 113,
					columnNumber: 11
				}, this)] }, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 108,
					columnNumber: 9
				}, this), /* @__PURE__ */ _jsxDEV("div", {
					className: "flex items-center bg-slate-900 border border-slate-700 rounded-lg p-1",
					children: [
						/* @__PURE__ */ _jsxDEV("button", {
							onClick: () => setActiveAnalysis("buffer"),
							className: `px-3 py-1.5 rounded text-xs font-semibold flex items-center gap-1.5 transition ${activeAnalysis === "buffer" ? "bg-emerald-600 text-white" : "text-slate-400 hover:text-white"}`,
							children: [/* @__PURE__ */ _jsxDEV(CircleDot, { className: "w-3.5 h-3.5" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 129,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Vùng Đệm (Buffer)" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 130,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 121,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("button", {
							onClick: () => setActiveAnalysis("knn"),
							className: `px-3 py-1.5 rounded text-xs font-semibold flex items-center gap-1.5 transition ${activeAnalysis === "knn" ? "bg-emerald-600 text-white" : "text-slate-400 hover:text-white"}`,
							children: [/* @__PURE__ */ _jsxDEV(Compass, { className: "w-3.5 h-3.5" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 140,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Tìm Đơn Vị Gần Nhất (K-NN)" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 141,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 132,
							columnNumber: 11
						}, this),
						/* @__PURE__ */ _jsxDEV("button", {
							onClick: () => setActiveAnalysis("sectors"),
							className: `px-3 py-1.5 rounded text-xs font-semibold flex items-center gap-1.5 transition ${activeAnalysis === "sectors" ? "bg-emerald-600 text-white" : "text-slate-400 hover:text-white"}`,
							children: [/* @__PURE__ */ _jsxDEV(Shield, { className: "w-3.5 h-3.5" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 151,
								columnNumber: 13
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "5 Khu Vực Phòng Thủ" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 152,
								columnNumber: 13
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 143,
							columnNumber: 11
						}, this)
					]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 120,
					columnNumber: 9
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 107,
				columnNumber: 7
			}, this),
			activeAnalysis === "buffer" && /* @__PURE__ */ _jsxDEV("div", {
				className: "space-y-6",
				children: [/* @__PURE__ */ _jsxDEV("div", {
					className: "bg-[#0e1628] border border-slate-800 rounded-xl p-5 shadow-md",
					children: [
						/* @__PURE__ */ _jsxDEV("div", {
							className: "text-xs font-bold uppercase text-amber-400 flex items-center gap-2 mb-3",
							children: [/* @__PURE__ */ _jsxDEV(CircleDot, { className: "w-4 h-4" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 163,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "CẤU HÌNH PHÂN TÍCH VÙNG ĐỆM PHÒNG THỦ (BUFFER ANALYSIS)" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 164,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 162,
							columnNumber: 13
						}, this),
						/* @__PURE__ */ _jsxDEV("div", {
							className: "grid grid-cols-1 md:grid-cols-3 gap-4",
							children: [
								/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
									className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
									children: "Chọn đơn vị làm tâm điểm:"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 169,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("select", {
									value: selectedCenterUnitId,
									onChange: (e) => setSelectedCenterUnitId(e.target.value),
									className: "w-full bg-[#090f1d] border border-slate-700 rounded-lg px-3 py-2 text-xs text-white focus:outline-none focus:border-amber-500",
									children: units.map((u) => /* @__PURE__ */ _jsxDEV("option", {
										value: u.id,
										children: [
											"[",
											u.code,
											"] ",
											u.name
										]
									}, u.id, true, {
										fileName: _jsxFileName,
										lineNumber: 178,
										columnNumber: 21
									}, this))
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 172,
									columnNumber: 17
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 168,
									columnNumber: 15
								}, this),
								/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
									className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
									children: "Bán kính bảo vệ / Tác chiến:"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 186,
									columnNumber: 17
								}, this), /* @__PURE__ */ _jsxDEV("div", {
									className: "flex items-center gap-2",
									children: /* @__PURE__ */ _jsxDEV("select", {
										value: bufferRadiusKm,
										onChange: (e) => setBufferRadiusKm(Number(e.target.value)),
										className: "flex-1 bg-[#090f1d] border border-slate-700 rounded-lg px-3 py-2 text-xs text-white font-mono focus:outline-none focus:border-amber-500",
										children: [
											/* @__PURE__ */ _jsxDEV("option", {
												value: 3,
												children: "3 km (Tầm súng cối / vành đai trực tiếp)"
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 195,
												columnNumber: 21
											}, this),
											/* @__PURE__ */ _jsxDEV("option", {
												value: 5,
												children: "5 km (Vành đai an ninh số 1)"
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 196,
												columnNumber: 21
											}, this),
											/* @__PURE__ */ _jsxDEV("option", {
												value: 10,
												children: "10 km (Vành đai phòng ngự chiến dịch)"
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 197,
												columnNumber: 21
											}, this),
											/* @__PURE__ */ _jsxDEV("option", {
												value: 15,
												children: "15 km (Tầm pháo binh cấp quân đoàn)"
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 198,
												columnNumber: 21
											}, this),
											/* @__PURE__ */ _jsxDEV("option", {
												value: 20,
												children: "20 km (Vùng bảo vệ pháo hỏa lực lớn)"
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 199,
												columnNumber: 21
											}, this),
											/* @__PURE__ */ _jsxDEV("option", {
												value: 30,
												children: "30 km (Phạm vi tên lửa phòng không tầm trung)"
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 200,
												columnNumber: 21
											}, this)
										]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 190,
										columnNumber: 19
									}, this)
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 189,
									columnNumber: 17
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 185,
									columnNumber: 15
								}, this),
								/* @__PURE__ */ _jsxDEV("div", {
									className: "flex items-end",
									children: /* @__PURE__ */ _jsxDEV("button", {
										onClick: () => {
											if (selectedCenterUnit) {
												onApplyBufferAndGoToMap({
													active: true,
													centerLat: selectedCenterUnit.latitude,
													centerLng: selectedCenterUnit.longitude,
													radiusKm: bufferRadiusKm,
													targetUnitName: selectedCenterUnit.name
												});
											}
										},
										className: "w-full bg-amber-600 hover:bg-amber-500 text-white font-semibold text-xs py-2 px-4 rounded-lg flex items-center justify-center gap-2 shadow-lg transition active:scale-95",
										children: [
											/* @__PURE__ */ _jsxDEV(CircleDot, { className: "w-4 h-4" }, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 220,
												columnNumber: 19
											}, this),
											/* @__PURE__ */ _jsxDEV("span", { children: "Vẽ Vùng Đệm Lên Bản Đồ GIS" }, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 221,
												columnNumber: 19
											}, this),
											/* @__PURE__ */ _jsxDEV(ArrowRight, { className: "w-4 h-4" }, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 222,
												columnNumber: 19
											}, this)
										]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 206,
										columnNumber: 17
									}, this)
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 205,
									columnNumber: 15
								}, this)
							]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 167,
							columnNumber: 13
						}, this),
						selectedCenterUnit && /* @__PURE__ */ _jsxDEV("div", {
							className: "mt-4 pt-3 border-t border-slate-800 text-xs text-slate-300 flex flex-wrap gap-4 font-mono",
							children: [
								/* @__PURE__ */ _jsxDEV("div", { children: ["Tâm điểm: ", /* @__PURE__ */ _jsxDEV("strong", {
									className: "text-white",
									children: selectedCenterUnit.name
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 230,
									columnNumber: 29
								}, this)] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 229,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("div", { children: [
									"Tọa độ WGS84: ",
									selectedCenterUnit.latitude.toFixed(5),
									"°N,",
									" ",
									selectedCenterUnit.longitude.toFixed(5),
									"°E"
								] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 232,
									columnNumber: 17
								}, this),
								/* @__PURE__ */ _jsxDEV("div", { children: [
									"Tọa độ VN-2000: X=",
									selectedCenterUnit.x_vn2000.toLocaleString(),
									"m, Y=",
									selectedCenterUnit.y_vn2000.toLocaleString(),
									"m"
								] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 236,
									columnNumber: 17
								}, this)
							]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 228,
							columnNumber: 15
						}, this)
					]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 161,
					columnNumber: 11
				}, this), /* @__PURE__ */ _jsxDEV("div", {
					className: "bg-[#0e1628] border border-slate-800 rounded-xl p-5 shadow-md space-y-4",
					children: [/* @__PURE__ */ _jsxDEV("div", {
						className: "flex items-center justify-between border-b border-slate-800 pb-3",
						children: [/* @__PURE__ */ _jsxDEV("h3", {
							className: "text-sm font-bold text-white uppercase tracking-wider flex items-center gap-2",
							children: /* @__PURE__ */ _jsxDEV("span", { children: [
								"Đơn vị nằm trong vùng đệm R = ",
								bufferRadiusKm,
								" km (",
								unitsWithinBuffer.length,
								" đơn vị)"
							] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 248,
								columnNumber: 17
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 247,
							columnNumber: 15
						}, this), /* @__PURE__ */ _jsxDEV("span", {
							className: "text-xs text-slate-400 font-mono",
							children: [
								"Diện tích vùng đệm: ",
								Math.round(Math.PI * bufferRadiusKm * bufferRadiusKm),
								" km²"
							]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 252,
							columnNumber: 15
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 246,
						columnNumber: 13
					}, this), unitsWithinBuffer.length === 0 ? /* @__PURE__ */ _jsxDEV("div", {
						className: "p-8 text-center text-slate-400 text-xs",
						children: "Không có đơn vị nào khác nằm trong phạm vi bán kính này. Hãy tăng bán kính vùng đệm."
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 258,
						columnNumber: 15
					}, this) : /* @__PURE__ */ _jsxDEV("div", {
						className: "grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3",
						children: unitsWithinBuffer.map(({ unit, distanceKm }) => {
							// Road mobility time at 40km/h
							const roadEstMinutes = Math.round(distanceKm * 1.35 * 60 / 40);
							return /* @__PURE__ */ _jsxDEV("div", {
								className: "p-3 bg-[#0a1120] border border-slate-800 rounded-lg hover:border-amber-500 transition group flex flex-col justify-between",
								children: [/* @__PURE__ */ _jsxDEV("div", { children: [
									/* @__PURE__ */ _jsxDEV("div", {
										className: "flex items-center justify-between mb-1.5",
										children: [/* @__PURE__ */ _jsxDEV("span", {
											className: "text-[10px] font-mono px-1.5 py-0.5 rounded bg-slate-800 text-amber-300 border border-slate-700 font-bold",
											children: unit.code
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 274,
											columnNumber: 27
										}, this), /* @__PURE__ */ _jsxDEV("span", {
											className: "text-xs font-mono font-bold text-amber-400",
											children: [
												"Cự ly: ",
												distanceKm,
												" km"
											]
										}, void 0, true, {
											fileName: _jsxFileName,
											lineNumber: 277,
											columnNumber: 27
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 273,
										columnNumber: 25
									}, this),
									/* @__PURE__ */ _jsxDEV("h4", {
										className: "text-xs font-bold text-white group-hover:text-amber-300 line-clamp-1",
										children: unit.name
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 281,
										columnNumber: 25
									}, this),
									/* @__PURE__ */ _jsxDEV("p", {
										className: "text-[11px] text-slate-400 mt-1 line-clamp-1",
										children: unit.address
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 284,
										columnNumber: 25
									}, this)
								] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 272,
									columnNumber: 23
								}, this), /* @__PURE__ */ _jsxDEV("div", {
									className: "mt-3 pt-2 border-t border-slate-800/80 flex items-center justify-between text-[11px]",
									children: [/* @__PURE__ */ _jsxDEV("span", {
										className: "flex items-center gap-1 text-slate-400",
										children: [/* @__PURE__ */ _jsxDEV(Clock, { className: "w-3 h-3 text-emerald-400" }, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 289,
											columnNumber: 27
										}, this), /* @__PURE__ */ _jsxDEV("span", { children: [
											"Cơ động ~",
											roadEstMinutes,
											" phút"
										] }, void 0, true, {
											fileName: _jsxFileName,
											lineNumber: 290,
											columnNumber: 27
										}, this)]
									}, void 0, true, {
										fileName: _jsxFileName,
										lineNumber: 288,
										columnNumber: 25
									}, this), /* @__PURE__ */ _jsxDEV("button", {
										onClick: () => onSelectUnitAndGoToMap(unit),
										className: "text-emerald-400 hover:underline font-semibold",
										children: "Xem vị trí →"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 292,
										columnNumber: 25
									}, this)]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 287,
									columnNumber: 23
								}, this)]
							}, unit.id, true, {
								fileName: _jsxFileName,
								lineNumber: 268,
								columnNumber: 21
							}, this);
						})
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 262,
						columnNumber: 15
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 245,
					columnNumber: 11
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 159,
				columnNumber: 9
			}, this),
			activeAnalysis === "knn" && /* @__PURE__ */ _jsxDEV("div", {
				className: "space-y-6",
				children: [/* @__PURE__ */ _jsxDEV("div", {
					className: "bg-[#0e1628] border border-slate-800 rounded-xl p-5 shadow-md",
					children: [/* @__PURE__ */ _jsxDEV("div", {
						className: "text-xs font-bold uppercase text-sky-400 flex items-center gap-2 mb-3",
						children: [/* @__PURE__ */ _jsxDEV(Compass, { className: "w-4 h-4" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 313,
							columnNumber: 15
						}, this), /* @__PURE__ */ _jsxDEV("span", { children: "TÌM KIẾM ĐƠN VỊ GẦN NHẤT (K-NEAREST NEIGHBORS QUERY)" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 314,
							columnNumber: 15
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 312,
						columnNumber: 13
					}, this), /* @__PURE__ */ _jsxDEV("div", {
						className: "grid grid-cols-1 md:grid-cols-3 gap-4",
						children: [
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Đơn vị / Vị trí yêu cầu chi viện:"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 319,
								columnNumber: 17
							}, this), /* @__PURE__ */ _jsxDEV("select", {
								value: knnTargetUnitId,
								onChange: (e) => setKnnTargetUnitId(e.target.value),
								className: "w-full bg-[#090f1d] border border-slate-700 rounded-lg px-3 py-2 text-xs text-white focus:outline-none focus:border-sky-500",
								children: units.map((u) => /* @__PURE__ */ _jsxDEV("option", {
									value: u.id,
									children: [
										"[",
										u.code,
										"] ",
										u.name
									]
								}, u.id, true, {
									fileName: _jsxFileName,
									lineNumber: 328,
									columnNumber: 21
								}, this))
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 322,
								columnNumber: 17
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 318,
								columnNumber: 15
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("label", {
								className: "text-[10px] text-slate-400 uppercase font-bold block mb-1",
								children: "Số lượng đơn vị gần nhất (K):"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 336,
								columnNumber: 17
							}, this), /* @__PURE__ */ _jsxDEV("select", {
								value: knnCount,
								onChange: (e) => setKnnCount(Number(e.target.value)),
								className: "w-full bg-[#090f1d] border border-slate-700 rounded-lg px-3 py-2 text-xs text-white font-mono focus:outline-none focus:border-sky-500",
								children: [
									/* @__PURE__ */ _jsxDEV("option", {
										value: 3,
										children: "Top 3 đơn vị gần nhất"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 344,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: 5,
										children: "Top 5 đơn vị gần nhất"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 345,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: 8,
										children: "Top 8 đơn vị gần nhất"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 346,
										columnNumber: 19
									}, this),
									/* @__PURE__ */ _jsxDEV("option", {
										value: 12,
										children: "Top 12 đơn vị gần nhất"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 347,
										columnNumber: 19
									}, this)
								]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 339,
								columnNumber: 17
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 335,
								columnNumber: 15
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "flex items-end",
								children: /* @__PURE__ */ _jsxDEV("div", {
									className: "p-2.5 rounded bg-sky-950/40 border border-sky-800 text-[11px] text-sky-300 w-full",
									children: "Phù hợp cho tình huống cứu hộ cứu nạn khẩn cấp, tác chiến A2, hoặc điều động hỏa lực chi viện nhanh nhất."
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 352,
									columnNumber: 17
								}, this)
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 351,
								columnNumber: 15
							}, this)
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 317,
						columnNumber: 13
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 311,
					columnNumber: 11
				}, this), /* @__PURE__ */ _jsxDEV("div", {
					className: "bg-[#0e1628] border border-slate-800 rounded-xl overflow-hidden shadow-md",
					children: [/* @__PURE__ */ _jsxDEV("div", {
						className: "p-4 border-b border-slate-800 flex justify-between items-center",
						children: [/* @__PURE__ */ _jsxDEV("h3", {
							className: "text-sm font-bold text-white uppercase tracking-wider",
							children: [
								"Kết quả ",
								knnCount,
								" đơn vị gần nhất so với \"",
								knnTargetUnit?.name,
								"\""
							]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 363,
							columnNumber: 15
						}, this), /* @__PURE__ */ _jsxDEV("span", {
							className: "text-xs text-emerald-400 font-mono",
							children: "Tính theo thuật toán khoảng cách Haversine & chiếu UTM"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 366,
							columnNumber: 15
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 362,
						columnNumber: 13
					}, this), /* @__PURE__ */ _jsxDEV("div", {
						className: "overflow-x-auto",
						children: /* @__PURE__ */ _jsxDEV("table", {
							className: "w-full text-left text-xs text-slate-300",
							children: [/* @__PURE__ */ _jsxDEV("thead", {
								className: "bg-[#090f1d] text-slate-400 uppercase text-[10px] tracking-wider font-mono border-b border-slate-800",
								children: /* @__PURE__ */ _jsxDEV("tr", { children: [
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-3",
										children: "Thứ tự"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 375,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-3",
										children: "Mã đơn vị"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 376,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-3",
										children: "Tên đơn vị chi viện"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 377,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-3",
										children: "Binh chủng"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 378,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-3",
										children: "Khoảng cách đường thẳng"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 379,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-3",
										children: "Ước tính thời gian cơ động"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 380,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-3",
										children: "Trực ban tác chiến"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 381,
										columnNumber: 21
									}, this),
									/* @__PURE__ */ _jsxDEV("th", {
										className: "p-3 text-right",
										children: "Xem GIS"
									}, void 0, false, {
										fileName: _jsxFileName,
										lineNumber: 382,
										columnNumber: 21
									}, this)
								] }, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 374,
									columnNumber: 19
								}, this)
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 373,
								columnNumber: 17
							}, this), /* @__PURE__ */ _jsxDEV("tbody", {
								className: "divide-y divide-slate-800",
								children: nearestResults.map(({ unit, distanceKm }, idx) => {
									const roadEstMinutes = Math.round(distanceKm * 1.35 * 60 / 40);
									return /* @__PURE__ */ _jsxDEV("tr", {
										className: "hover:bg-slate-800/60 transition",
										children: [
											/* @__PURE__ */ _jsxDEV("td", {
												className: "p-3 font-mono font-bold text-sky-400",
												children: ["#", idx + 1]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 390,
												columnNumber: 25
											}, this),
											/* @__PURE__ */ _jsxDEV("td", {
												className: "p-3 font-mono text-emerald-400 font-semibold",
												children: unit.code
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 391,
												columnNumber: 25
											}, this),
											/* @__PURE__ */ _jsxDEV("td", {
												className: "p-3 font-bold text-white",
												children: unit.name
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 392,
												columnNumber: 25
											}, this),
											/* @__PURE__ */ _jsxDEV("td", {
												className: "p-3",
												children: unit.branch
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 393,
												columnNumber: 25
											}, this),
											/* @__PURE__ */ _jsxDEV("td", {
												className: "p-3 font-mono text-amber-300 font-bold",
												children: [
													distanceKm,
													" km (",
													Math.round(distanceKm * 1e3).toLocaleString(),
													" m)"
												]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 394,
												columnNumber: 25
											}, this),
											/* @__PURE__ */ _jsxDEV("td", {
												className: "p-3 font-mono text-emerald-300",
												children: [
													"~",
													roadEstMinutes,
													" phút (40 km/h)"
												]
											}, void 0, true, {
												fileName: _jsxFileName,
												lineNumber: 397,
												columnNumber: 25
											}, this),
											/* @__PURE__ */ _jsxDEV("td", {
												className: "p-3 font-mono",
												children: unit.phone
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 400,
												columnNumber: 25
											}, this),
											/* @__PURE__ */ _jsxDEV("td", {
												className: "p-3 text-right",
												children: /* @__PURE__ */ _jsxDEV("button", {
													onClick: () => onSelectUnitAndGoToMap(unit),
													className: "bg-sky-700/80 hover:bg-sky-600 text-white px-2.5 py-1 rounded text-[11px] font-semibold",
													children: "Định vị"
												}, void 0, false, {
													fileName: _jsxFileName,
													lineNumber: 402,
													columnNumber: 27
												}, this)
											}, void 0, false, {
												fileName: _jsxFileName,
												lineNumber: 401,
												columnNumber: 25
											}, this)
										]
									}, unit.id, true, {
										fileName: _jsxFileName,
										lineNumber: 389,
										columnNumber: 23
									}, this);
								})
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 385,
								columnNumber: 17
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 372,
							columnNumber: 15
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 371,
						columnNumber: 13
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 361,
					columnNumber: 11
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 310,
				columnNumber: 9
			}, this),
			activeAnalysis === "sectors" && /* @__PURE__ */ _jsxDEV("div", {
				className: "space-y-6",
				children: [/* @__PURE__ */ _jsxDEV("div", {
					className: "p-4 rounded-xl bg-[#0e1628] border border-slate-800 text-xs text-slate-300 space-y-2",
					children: [/* @__PURE__ */ _jsxDEV("h3", {
						className: "text-sm font-bold text-white uppercase flex items-center gap-2",
						children: [/* @__PURE__ */ _jsxDEV(Shield, { className: "w-4 h-4 text-emerald-400" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 424,
							columnNumber: 15
						}, this), /* @__PURE__ */ _jsxDEV("span", { children: "5 Khu vực Phòng thủ Chiến lược của Thành phố Hà Nội" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 425,
							columnNumber: 15
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 423,
						columnNumber: 13
					}, this), /* @__PURE__ */ _jsxDEV("p", { children: "Bố trí không gian phòng thủ phân bổ theo 5 hướng then chốt nhằm ngăn chặn mọi hướng tiến công, bảo vệ an ninh trật tự và sẵn sàng cơ động phòng chống thiên tai lụt bão." }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 427,
						columnNumber: 13
					}, this)]
				}, void 0, true, {
					fileName: _jsxFileName,
					lineNumber: 422,
					columnNumber: 11
				}, this), /* @__PURE__ */ _jsxDEV("div", {
					className: "grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4",
					children: Object.entries(sectorBreakdown).map(([sectorName, data]) => /* @__PURE__ */ _jsxDEV("div", {
						className: "bg-[#0e1628] border border-slate-800 rounded-xl p-4 shadow-md space-y-3",
						children: [
							/* @__PURE__ */ _jsxDEV("div", {
								className: "flex items-center justify-between border-b border-slate-800 pb-2",
								children: [/* @__PURE__ */ _jsxDEV("h4", {
									className: "text-xs font-bold text-white uppercase",
									children: sectorName
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 440,
									columnNumber: 19
								}, this), /* @__PURE__ */ _jsxDEV("span", {
									className: "text-xs font-mono px-2 py-0.5 rounded bg-emerald-950 text-emerald-300 border border-emerald-800 font-bold",
									children: [data.count, " đơn vị"]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 441,
									columnNumber: 19
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 439,
								columnNumber: 17
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("div", {
								className: "text-[10px] text-slate-400 uppercase font-bold mb-1",
								children: [
									"Các quận/huyện trực thuộc (",
									data.districts.length,
									"):"
								]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 447,
								columnNumber: 19
							}, this), /* @__PURE__ */ _jsxDEV("div", {
								className: "flex flex-wrap gap-1",
								children: data.districts.map((d) => /* @__PURE__ */ _jsxDEV("span", {
									className: "px-1.5 py-0.5 rounded bg-slate-900 border border-slate-800 text-[10px] text-slate-300",
									children: d
								}, d, false, {
									fileName: _jsxFileName,
									lineNumber: 452,
									columnNumber: 23
								}, this))
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 450,
								columnNumber: 19
							}, this)] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 446,
								columnNumber: 17
							}, this),
							/* @__PURE__ */ _jsxDEV("div", {
								className: "pt-2 border-t border-slate-800/80",
								children: [/* @__PURE__ */ _jsxDEV("div", {
									className: "text-[10px] text-slate-400 uppercase font-bold mb-1",
									children: "Một số đơn vị nòng cốt:"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 463,
									columnNumber: 19
								}, this), /* @__PURE__ */ _jsxDEV("ul", {
									className: "text-xs text-slate-300 space-y-1",
									children: data.units.slice(0, 3).map((u) => /* @__PURE__ */ _jsxDEV("li", {
										className: "flex items-center justify-between",
										children: [/* @__PURE__ */ _jsxDEV("span", {
											className: "truncate pr-2",
											children: ["• ", u.shortName]
										}, void 0, true, {
											fileName: _jsxFileName,
											lineNumber: 469,
											columnNumber: 25
										}, this), /* @__PURE__ */ _jsxDEV("span", {
											className: "text-[10px] text-emerald-400 font-mono flex-shrink-0",
											children: u.branch
										}, void 0, false, {
											fileName: _jsxFileName,
											lineNumber: 470,
											columnNumber: 25
										}, this)]
									}, u.id, true, {
										fileName: _jsxFileName,
										lineNumber: 468,
										columnNumber: 23
									}, this))
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 466,
									columnNumber: 19
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 462,
								columnNumber: 17
							}, this)
						]
					}, sectorName, true, {
						fileName: _jsxFileName,
						lineNumber: 435,
						columnNumber: 15
					}, this))
				}, void 0, false, {
					fileName: _jsxFileName,
					lineNumber: 433,
					columnNumber: 11
				}, this)]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 421,
				columnNumber: 9
			}, this)
		]
	}, void 0, true, {
		fileName: _jsxFileName,
		lineNumber: 105,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,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SyxXQUFVO2tCQUFmO1NBQXFFO1NBQ3ZDLEtBQUssVUFBVTtTQUFPO1FBQy9DOzs7OztpQkFDTCx3QkFBQyxPQUFEO1FBQUssV0FBVTtrQkFDWixLQUFLLFVBQVUsS0FBSyxNQUNuQix3QkFBQyxRQUFEO1NBRUUsV0FBVTttQkFFVDtRQUNHLEdBSkM7Ozs7ZUFJRCxDQUNQO09BQ0U7Ozs7ZUFDRjs7Ozs7T0FFTCx3QkFBQyxPQUFEO1FBQUssV0FBVTtrQkFBZixDQUNFLHdCQUFDLE9BQUQ7U0FBSyxXQUFVO21CQUFzRDtRQUVoRTs7OztrQkFDTCx3QkFBQyxNQUFEO1NBQUksV0FBVTttQkFDWCxLQUFLLE1BQU0sTUFBTSxHQUFHLENBQUMsQ0FBQyxDQUFDLEtBQUssTUFDM0Isd0JBQUMsTUFBRDtVQUFlLFdBQVU7b0JBQXpCLENBQ0Usd0JBQUMsUUFBRDtXQUFNLFdBQVU7cUJBQWhCLENBQWdDLE1BQUcsRUFBRSxTQUFnQjs7Ozs7b0JBQ3JELHdCQUFDLFFBQUQ7V0FBTSxXQUFVO3FCQUNiLEVBQUU7VUFDQzs7OztrQkFDSjtZQUxLLEVBQUU7Ozs7Z0JBS1AsQ0FDTDtRQUNDOzs7O2dCQUNEOzs7Ozs7TUFDRjtRQXpDRTs7OztZQXlDRixDQUNOO0lBQ0U7Ozs7WUFDRjs7Ozs7O0VBRUo7Ozs7OztBQUVUIiwibmFtZXMiOltdLCJzb3VyY2VzIjpbIlNwYXRpYWxBbmFseXNpc1ZpZXcudHN4Il0sInZlcnNpb24iOjMsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCBSZWFjdCwgeyB1c2VTdGF0ZSwgdXNlTWVtbyB9IGZyb20gJ3JlYWN0JztcbmltcG9ydCB7IE1pbGl0YXJ5VW5pdCwgQnVmZmVyQW5hbHlzaXMgfSBmcm9tICcuLi90eXBlcy9naXMnO1xuaW1wb3J0IHsgY2FsY3VsYXRlRGlzdGFuY2VLbSB9IGZyb20gJy4uL3V0aWxzL2Nvb3JkaW5hdGVDb252ZXJzaW9uJztcbmltcG9ydCB7IGZpbmROZWFyZXN0VW5pdHMgfSBmcm9tICcuLi91dGlscy9naXNDYWxjdWxhdGlvbnMnO1xuaW1wb3J0IHsgSEFOT0lfRElTVFJJQ1RTIH0gZnJvbSAnLi4vZGF0YS9oYW5vaURpc3RyaWN0cyc7XG5pbXBvcnQge1xuICBDcm9zc2hhaXIsXG4gIENpcmNsZURvdCxcbiAgQ29tcGFzcyxcbiAgQXJyb3dSaWdodCxcbiAgU2hpZWxkLFxuICBMYXllcnMsXG4gIE1hcFBpbixcbiAgQ2xvY2ssXG4gIFphcCxcbn0gZnJvbSAnbHVjaWRlLXJlYWN0JztcblxuaW50ZXJmYWNlIFNwYXRpYWxBbmFseXNpc1ZpZXdQcm9wcyB7XG4gIHVuaXRzOiBNaWxpdGFyeVVuaXRbXTtcbiAgb25BcHBseUJ1ZmZlckFuZEdvVG9NYXA6IChidWZmZXI6IEJ1ZmZlckFuYWx5c2lzKSA9PiB2b2lkO1xuICBvblNlbGVjdFVuaXRBbmRHb1RvTWFwOiAodW5pdDogTWlsaXRhcnlVbml0KSA9PiB2b2lkO1xufVxuXG5leHBvcnQgY29uc3QgU3BhdGlhbEFuYWx5c2lzVmlldzogUmVhY3QuRkM8U3BhdGlhbEFuYWx5c2lzVmlld1Byb3BzPiA9ICh7XG4gIHVuaXRzLFxuICBvbkFwcGx5QnVmZmVyQW5kR29Ub01hcCxcbiAgb25TZWxlY3RVbml0QW5kR29Ub01hcCxcbn0pID0+IHtcbiAgLy8gVGFiIHdpdGhpbiBzcGF0aWFsIGFuYWx5c2lzXG4gIGNvbnN0IFthY3RpdmVBbmFseXNpcywgc2V0QWN0aXZlQW5hbHlzaXNdID0gdXNlU3RhdGU8J2J1ZmZlcicgfCAna25uJyB8ICdzZWN0b3JzJz4oJ2J1ZmZlcicpO1xuXG4gIC8vIEJ1ZmZlciBzdGF0ZVxuICBjb25zdCBbc2VsZWN0ZWRDZW50ZXJVbml0SWQsIHNldFNlbGVjdGVkQ2VudGVyVW5pdElkXSA9IHVzZVN0YXRlPHN0cmluZz4odW5pdHNbMF0/LmlkIHx8ICcnKTtcbiAgY29uc3QgW2J1ZmZlclJhZGl1c0ttLCBzZXRCdWZmZXJSYWRpdXNLbV0gPSB1c2VTdGF0ZTxudW1iZXI+KDEwKTtcblxuICAvLyBLTk4gc3RhdGVcbiAgY29uc3QgW2tublRhcmdldFVuaXRJZCwgc2V0S25uVGFyZ2V0VW5pdElkXSA9IHVzZVN0YXRlPHN0cmluZz4odW5pdHNbMF0/LmlkIHx8ICcnKTtcbiAgY29uc3QgW2tubkNvdW50LCBzZXRLbm5Db3VudF0gPSB1c2VTdGF0ZTxudW1iZXI+KDUpO1xuXG4gIGNvbnN0IHNlbGVjdGVkQ2VudGVyVW5pdCA9IHVzZU1lbW8oXG4gICAgKCkgPT4gdW5pdHMuZmluZCgodSkgPT4gdS5pZCA9PT0gc2VsZWN0ZWRDZW50ZXJVbml0SWQpIHx8IHVuaXRzWzBdLFxuICAgIFt1bml0cywgc2VsZWN0ZWRDZW50ZXJVbml0SWRdXG4gICk7XG5cbiAgY29uc3Qga25uVGFyZ2V0VW5pdCA9IHVzZU1lbW8oXG4gICAgKCkgPT4gdW5pdHMuZmluZCgodSkgPT4gdS5pZCA9PT0ga25uVGFyZ2V0VW5pdElkKSB8fCB1bml0c1swXSxcbiAgICBbdW5pdHMsIGtublRhcmdldFVuaXRJZF1cbiAgKTtcblxuICAvLyBVbml0cyB3aXRoaW4gYnVmZmVyIGNhbGN1bGF0aW9uXG4gIGNvbnN0IHVuaXRzV2l0aGluQnVmZmVyID0gdXNlTWVtbygoKSA9PiB7XG4gICAgaWYgKCFzZWxlY3RlZENlbnRlclVuaXQpIHJldHVybiBbXTtcbiAgICByZXR1cm4gdW5pdHNcbiAgICAgIC5tYXAoKHUpID0+IHtcbiAgICAgICAgY29uc3QgZGlzdCA9IGNhbGN1bGF0ZURpc3RhbmNlS20oXG4gICAgICAgICAgc2VsZWN0ZWRDZW50ZXJVbml0LmxhdGl0dWRlLFxuICAgICAgICAgIHNlbGVjdGVkQ2VudGVyVW5pdC5sb25naXR1ZGUsXG4gICAgICAgICAgdS5sYXRpdHVkZSxcbiAgICAgICAgICB1LmxvbmdpdHVkZVxuICAgICAgICApO1xuICAgICAgICByZXR1cm4geyB1bml0OiB1LCBkaXN0YW5jZUttOiBkaXN0IH07XG4gICAgICB9KVxuICAgICAgLmZpbHRlcigoaXRlbSkgPT4gaXRlbS5kaXN0YW5jZUttIDw9IGJ1ZmZlclJhZGl1c0ttICYmIGl0ZW0udW5pdC5pZCAhPT0gc2VsZWN0ZWRDZW50ZXJVbml0LmlkKVxuICAgICAgLnNvcnQoKGEsIGIpID0+IGEuZGlzdGFuY2VLbSAtIGIuZGlzdGFuY2VLbSk7XG4gIH0sIFt1bml0cywgc2VsZWN0ZWRDZW50ZXJVbml0LCBidWZmZXJSYWRpdXNLbV0pO1xuXG4gIC8vIEtOTiBjYWxjdWxhdGlvblxuICBjb25zdCBuZWFyZXN0UmVzdWx0cyA9IHVzZU1lbW8oKCkgPT4ge1xuICAgIGlmICgha25uVGFyZ2V0VW5pdCkgcmV0dXJuIFtdO1xuICAgIGNvbnN0IGNhbmRpZGF0ZXMgPSB1bml0cy5maWx0ZXIoKHUpID0+IHUuaWQgIT09IGtublRhcmdldFVuaXQuaWQpO1xuICAgIHJldHVybiBmaW5kTmVhcmVzdFVuaXRzKGtublRhcmdldFVuaXQubGF0aXR1ZGUsIGtublRhcmdldFVuaXQubG9uZ2l0dWRlLCBjYW5kaWRhdGVzLCBrbm5Db3VudCk7XG4gIH0sIFt1bml0cywga25uVGFyZ2V0VW5pdCwga25uQ291bnRdKTtcblxuICAvLyBTZWN0b3IgYnJlYWtkb3duXG4gIGNvbnN0IHNlY3RvckJyZWFrZG93biA9IHVzZU1lbW8oKCkgPT4ge1xuICAgIGNvbnN0IHNlY3RvcnM6IFJlY29yZDxzdHJpbmcsIHsgY291bnQ6IG51bWJlcjsgdW5pdHM6IE1pbGl0YXJ5VW5pdFtdOyBkaXN0cmljdHM6IHN0cmluZ1tdIH0+ID0ge1xuICAgICAgJ0todSB24buxYyBUcnVuZyB0w6JtJzogeyBjb3VudDogMCwgdW5pdHM6IFtdLCBkaXN0cmljdHM6IFtdIH0sXG4gICAgICAnS2h1IHbhu7FjIFBow61hIFTDonknOiB7IGNvdW50OiAwLCB1bml0czogW10sIGRpc3RyaWN0czogW10gfSxcbiAgICAgICdLaHUgduG7sWMgUGjDrWEgQuG6r2MnOiB7IGNvdW50OiAwLCB1bml0czogW10sIGRpc3RyaWN0czogW10gfSxcbiAgICAgICdLaHUgduG7sWMgUGjDrWEgTmFtJzogeyBjb3VudDogMCwgdW5pdHM6IFtdLCBkaXN0cmljdHM6IFtdIH0sXG4gICAgICAnS2h1IHbhu7FjIFBow61hIMSQw7RuZyc6IHsgY291bnQ6IDAsIHVuaXRzOiBbXSwgZGlzdHJpY3RzOiBbXSB9LFxuICAgIH07XG5cbiAgICAvLyBNYXAgZGlzdHJpY3RzIHRvIHNlY3RvcnNcbiAgICBjb25zdCBkaXN0cmljdFNlY3Rvck1hcCA9IG5ldyBNYXA8c3RyaW5nLCBzdHJpbmc+KCk7XG4gICAgSEFOT0lfRElTVFJJQ1RTLmZvckVhY2goKGQpID0+IHtcbiAgICAgIGRpc3RyaWN0U2VjdG9yTWFwLnNldChkLm5hbWUsIGQuZGVmZW5zaXZlU2VjdG9yKTtcbiAgICAgIGlmIChzZWN0b3JzW2QuZGVmZW5zaXZlU2VjdG9yXSAmJiAhc2VjdG9yc1tkLmRlZmVuc2l2ZVNlY3Rvcl0uZGlzdHJpY3RzLmluY2x1ZGVzKGQubmFtZSkpIHtcbiAgICAgICAgc2VjdG9yc1tkLmRlZmVuc2l2ZVNlY3Rvcl0uZGlzdHJpY3RzLnB1c2goZC5zaG9ydE5hbWUpO1xuICAgICAgfVxuICAgIH0pO1xuXG4gICAgdW5pdHMuZm9yRWFjaCgodSkgPT4ge1xuICAgICAgY29uc3Qgc2VjID0gZGlzdHJpY3RTZWN0b3JNYXAuZ2V0KHUuZGlzdHJpY3QpIHx8ICdLaHUgduG7sWMgVHJ1bmcgdMOibSc7XG4gICAgICBpZiAoc2VjdG9yc1tzZWNdKSB7XG4gICAgICAgIHNlY3RvcnNbc2VjXS5jb3VudCsrO1xuICAgICAgICBzZWN0b3JzW3NlY10udW5pdHMucHVzaCh1KTtcbiAgICAgIH1cbiAgICB9KTtcblxuICAgIHJldHVybiBzZWN0b3JzO1xuICB9LCBbdW5pdHNdKTtcblxuICByZXR1cm4gKFxuICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleC0xIG92ZXJmbG93LXktYXV0byBiZy1bIzA4MGQxOV0gdGV4dC13aGl0ZSBwLTYgc3BhY2UteS02XCI+XG4gICAgICB7LyogVGl0bGUgKi99XG4gICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggZmxleC13cmFwIGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gZ2FwLTQgYm9yZGVyLWIgYm9yZGVyLXNsYXRlLTgwMCBwYi00XCI+XG4gICAgICAgIDxkaXY+XG4gICAgICAgICAgPGgyIGNsYXNzTmFtZT1cInRleHQteGwgZm9udC1ib2xkIHVwcGVyY2FzZSB0cmFja2luZy13aWRlciB0ZXh0LXdoaXRlIGZsZXggaXRlbXMtY2VudGVyIGdhcC0yXCI+XG4gICAgICAgICAgICA8Q3Jvc3NoYWlyIGNsYXNzTmFtZT1cInctNSBoLTUgdGV4dC1lbWVyYWxkLTQwMFwiIC8+XG4gICAgICAgICAgICA8c3Bhbj5QaMOibiB0w61jaCBLaMO0bmcgZ2lhbiBRdcOibiBz4buxIEdJUyAoU3BhdGlhbCBBbmFseXNpcyk8L3NwYW4+XG4gICAgICAgICAgPC9oMj5cbiAgICAgICAgICA8cCBjbGFzc05hbWU9XCJ0ZXh0LXhzIHRleHQtc2xhdGUtNDAwIG10LTFcIj5cbiAgICAgICAgICAgIOG7qG5nIGThu6VuZyBjw6FjIHRodeG6rXQgdG/DoW4gR0lTIGtow7RuZyBnaWFuOiBWw7luZyDEkeG7h20gYsOhbiBrw61uaCAoQnVmZmVyIFpvbmUpLCBLLU5OIMSRxqFuIHbhu4sgZ+G6p25cbiAgICAgICAgICAgIG5o4bqldCB2w6AgcGjDom4gYuG7lSB0aOG6vyB0cuG6rW4gcGjDsm5nIHRo4bunXG4gICAgICAgICAgPC9wPlxuICAgICAgICA8L2Rpdj5cblxuICAgICAgICB7LyogTW9kZSBTZWxlY3RvciBUYWJzICovfVxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGJnLXNsYXRlLTkwMCBib3JkZXIgYm9yZGVyLXNsYXRlLTcwMCByb3VuZGVkLWxnIHAtMVwiPlxuICAgICAgICAgIDxidXR0b25cbiAgICAgICAgICAgIG9uQ2xpY2s9eygpID0+IHNldEFjdGl2ZUFuYWx5c2lzKCdidWZmZXInKX1cbiAgICAgICAgICAgIGNsYXNzTmFtZT17YHB4LTMgcHktMS41IHJvdW5kZWQgdGV4dC14cyBmb250LXNlbWlib2xkIGZsZXggaXRlbXMtY2VudGVyIGdhcC0xLjUgdHJhbnNpdGlvbiAke1xuICAgICAgICAgICAgICBhY3RpdmVBbmFseXNpcyA9PT0gJ2J1ZmZlcidcbiAgICAgICAgICAgICAgICA/ICdiZy1lbWVyYWxkLTYwMCB0ZXh0LXdoaXRlJ1xuICAgICAgICAgICAgICAgIDogJ3RleHQtc2xhdGUtNDAwIGhvdmVyOnRleHQtd2hpdGUnXG4gICAgICAgICAgICB9YH1cbiAgICAgICAgICA+XG4gICAgICAgICAgICA8Q2lyY2xlRG90IGNsYXNzTmFtZT1cInctMy41IGgtMy41XCIgLz5cbiAgICAgICAgICAgIDxzcGFuPlbDuW5nIMSQ4buHbSAoQnVmZmVyKTwvc3Bhbj5cbiAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiBzZXRBY3RpdmVBbmFseXNpcygna25uJyl9XG4gICAgICAgICAgICBjbGFzc05hbWU9e2BweC0zIHB5LTEuNSByb3VuZGVkIHRleHQteHMgZm9udC1zZW1pYm9sZCBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMS41IHRyYW5zaXRpb24gJHtcbiAgICAgICAgICAgICAgYWN0aXZlQW5hbHlzaXMgPT09ICdrbm4nXG4gICAgICAgICAgICAgICAgPyAnYmctZW1lcmFsZC02MDAgdGV4dC13aGl0ZSdcbiAgICAgICAgICAgICAgICA6ICd0ZXh0LXNsYXRlLTQwMCBob3Zlcjp0ZXh0LXdoaXRlJ1xuICAgICAgICAgICAgfWB9XG4gICAgICAgICAgPlxuICAgICAgICAgICAgPENvbXBhc3MgY2xhc3NOYW1lPVwidy0zLjUgaC0zLjVcIiAvPlxuICAgICAgICAgICAgPHNwYW4+VMOsbSDEkMahbiBW4buLIEfhuqduIE5o4bqldCAoSy1OTik8L3NwYW4+XG4gICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgPGJ1dHRvblxuICAgICAgICAgICAgb25DbGljaz17KCkgPT4gc2V0QWN0aXZlQW5hbHlzaXMoJ3NlY3RvcnMnKX1cbiAgICAgICAgICAgIGNsYXNzTmFtZT17YHB4LTMgcHktMS41IHJvdW5kZWQgdGV4dC14cyBmb250LXNlbWlib2xkIGZsZXggaXRlbXMtY2VudGVyIGdhcC0xLjUgdHJhbnNpdGlvbiAke1xuICAgICAgICAgICAgICBhY3RpdmVBbmFseXNpcyA9PT0gJ3NlY3RvcnMnXG4gICAgICAgICAgICAgICAgPyAnYmctZW1lcmFsZC02MDAgdGV4dC13aGl0ZSdcbiAgICAgICAgICAgICAgICA6ICd0ZXh0LXNsYXRlLTQwMCBob3Zlcjp0ZXh0LXdoaXRlJ1xuICAgICAgICAgICAgfWB9XG4gICAgICAgICAgPlxuICAgICAgICAgICAgPFNoaWVsZCBjbGFzc05hbWU9XCJ3LTMuNSBoLTMuNVwiIC8+XG4gICAgICAgICAgICA8c3Bhbj41IEtodSBW4buxYyBQaMOybmcgVGjhu6c8L3NwYW4+XG4gICAgICAgICAgPC9idXR0b24+XG4gICAgICAgIDwvZGl2PlxuICAgICAgPC9kaXY+XG5cbiAgICAgIHsvKiAxLiBCVUZGRVIgQU5BTFlTSVMgKi99XG4gICAgICB7YWN0aXZlQW5hbHlzaXMgPT09ICdidWZmZXInICYmIChcbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJzcGFjZS15LTZcIj5cbiAgICAgICAgICB7LyogQ29udHJvbHMgQmFyICovfVxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHJvdW5kZWQteGwgcC01IHNoYWRvdy1tZFwiPlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LXhzIGZvbnQtYm9sZCB1cHBlcmNhc2UgdGV4dC1hbWJlci00MDAgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTIgbWItM1wiPlxuICAgICAgICAgICAgICA8Q2lyY2xlRG90IGNsYXNzTmFtZT1cInctNCBoLTRcIiAvPlxuICAgICAgICAgICAgICA8c3Bhbj5D4bqkVSBIw4xOSCBQSMOCTiBUw41DSCBWw5lORyDEkOG7hk0gUEjDkk5HIFRI4bumIChCVUZGRVIgQU5BTFlTSVMpPC9zcGFuPlxuICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZ3JpZCBncmlkLWNvbHMtMSBtZDpncmlkLWNvbHMtMyBnYXAtNFwiPlxuICAgICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICAgIDxsYWJlbCBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIGJsb2NrIG1iLTFcIj5cbiAgICAgICAgICAgICAgICAgIENo4buNbiDEkcahbiB24buLIGzDoG0gdMOibSDEkWnhu4NtOlxuICAgICAgICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgICAgICAgICAgPHNlbGVjdFxuICAgICAgICAgICAgICAgICAgdmFsdWU9e3NlbGVjdGVkQ2VudGVyVW5pdElkfVxuICAgICAgICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PiBzZXRTZWxlY3RlZENlbnRlclVuaXRJZChlLnRhcmdldC52YWx1ZSl9XG4gICAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJ3LWZ1bGwgYmctWyMwOTBmMWRdIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIHJvdW5kZWQtbGcgcHgtMyBweS0yIHRleHQteHMgdGV4dC13aGl0ZSBmb2N1czpvdXRsaW5lLW5vbmUgZm9jdXM6Ym9yZGVyLWFtYmVyLTUwMFwiXG4gICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAge3VuaXRzLm1hcCgodSkgPT4gKFxuICAgICAgICAgICAgICAgICAgICA8b3B0aW9uIGtleT17dS5pZH0gdmFsdWU9e3UuaWR9PlxuICAgICAgICAgICAgICAgICAgICAgIFt7dS5jb2RlfV0ge3UubmFtZX1cbiAgICAgICAgICAgICAgICAgICAgPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgICApKX1cbiAgICAgICAgICAgICAgICA8L3NlbGVjdD5cbiAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgICA8bGFiZWwgY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGZvbnQtYm9sZCBibG9jayBtYi0xXCI+XG4gICAgICAgICAgICAgICAgICBCw6FuIGvDrW5oIGLhuqNvIHbhu4cgLyBUw6FjIGNoaeG6v246XG4gICAgICAgICAgICAgICAgPC9sYWJlbD5cbiAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGdhcC0yXCI+XG4gICAgICAgICAgICAgICAgICA8c2VsZWN0XG4gICAgICAgICAgICAgICAgICAgIHZhbHVlPXtidWZmZXJSYWRpdXNLbX1cbiAgICAgICAgICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PiBzZXRCdWZmZXJSYWRpdXNLbShOdW1iZXIoZS50YXJnZXQudmFsdWUpKX1cbiAgICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwiZmxleC0xIGJnLVsjMDkwZjFkXSBib3JkZXIgYm9yZGVyLXNsYXRlLTcwMCByb3VuZGVkLWxnIHB4LTMgcHktMiB0ZXh0LXhzIHRleHQtd2hpdGUgZm9udC1tb25vIGZvY3VzOm91dGxpbmUtbm9uZSBmb2N1czpib3JkZXItYW1iZXItNTAwXCJcbiAgICAgICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT17M30+MyBrbSAoVOG6p20gc8O6bmcgY+G7kWkgLyB2w6BuaCDEkWFpIHRy4buxYyB0aeG6v3ApPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9ezV9PjUga20gKFbDoG5oIMSRYWkgYW4gbmluaCBz4buRIDEpPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9ezEwfT4xMCBrbSAoVsOgbmggxJFhaSBwaMOybmcgbmfhu7EgY2hp4bq/biBk4buLY2gpPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9ezE1fT4xNSBrbSAoVOG6p20gcGjDoW8gYmluaCBj4bqlcCBxdcOibiDEkW/DoG4pPC9vcHRpb24+XG4gICAgICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9ezIwfT4yMCBrbSAoVsO5bmcgYuG6o28gduG7hyBwaMOhbyBo4buPYSBs4buxYyBs4bubbik8L29wdGlvbj5cbiAgICAgICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT17MzB9PjMwIGttIChQaOG6oW0gdmkgdMOqbiBs4butYSBwaMOybmcga2jDtG5nIHThuqdtIHRydW5nKTwvb3B0aW9uPlxuICAgICAgICAgICAgICAgICAgPC9zZWxlY3Q+XG4gICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1lbmRcIj5cbiAgICAgICAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiB7XG4gICAgICAgICAgICAgICAgICAgIGlmIChzZWxlY3RlZENlbnRlclVuaXQpIHtcbiAgICAgICAgICAgICAgICAgICAgICBvbkFwcGx5QnVmZmVyQW5kR29Ub01hcCh7XG4gICAgICAgICAgICAgICAgICAgICAgICBhY3RpdmU6IHRydWUsXG4gICAgICAgICAgICAgICAgICAgICAgICBjZW50ZXJMYXQ6IHNlbGVjdGVkQ2VudGVyVW5pdC5sYXRpdHVkZSxcbiAgICAgICAgICAgICAgICAgICAgICAgIGNlbnRlckxuZzogc2VsZWN0ZWRDZW50ZXJVbml0LmxvbmdpdHVkZSxcbiAgICAgICAgICAgICAgICAgICAgICAgIHJhZGl1c0ttOiBidWZmZXJSYWRpdXNLbSxcbiAgICAgICAgICAgICAgICAgICAgICAgIHRhcmdldFVuaXROYW1lOiBzZWxlY3RlZENlbnRlclVuaXQubmFtZSxcbiAgICAgICAgICAgICAgICAgICAgICB9KTtcbiAgICAgICAgICAgICAgICAgICAgfVxuICAgICAgICAgICAgICAgICAgfX1cbiAgICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1hbWJlci02MDAgaG92ZXI6YmctYW1iZXItNTAwIHRleHQtd2hpdGUgZm9udC1zZW1pYm9sZCB0ZXh0LXhzIHB5LTIgcHgtNCByb3VuZGVkLWxnIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktY2VudGVyIGdhcC0yIHNoYWRvdy1sZyB0cmFuc2l0aW9uIGFjdGl2ZTpzY2FsZS05NVwiXG4gICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgPENpcmNsZURvdCBjbGFzc05hbWU9XCJ3LTQgaC00XCIgLz5cbiAgICAgICAgICAgICAgICAgIDxzcGFuPlbhur0gVsO5bmcgxJDhu4dtIEzDqm4gQuG6o24gxJDhu5MgR0lTPC9zcGFuPlxuICAgICAgICAgICAgICAgICAgPEFycm93UmlnaHQgY2xhc3NOYW1lPVwidy00IGgtNFwiIC8+XG4gICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgIHtzZWxlY3RlZENlbnRlclVuaXQgJiYgKFxuICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cIm10LTQgcHQtMyBib3JkZXItdCBib3JkZXItc2xhdGUtODAwIHRleHQteHMgdGV4dC1zbGF0ZS0zMDAgZmxleCBmbGV4LXdyYXAgZ2FwLTQgZm9udC1tb25vXCI+XG4gICAgICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgICAgIFTDom0gxJFp4buDbTogPHN0cm9uZyBjbGFzc05hbWU9XCJ0ZXh0LXdoaXRlXCI+e3NlbGVjdGVkQ2VudGVyVW5pdC5uYW1lfTwvc3Ryb25nPlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgICAgICBU4buNYSDEkeG7mSBXR1M4NDoge3NlbGVjdGVkQ2VudGVyVW5pdC5sYXRpdHVkZS50b0ZpeGVkKDUpfcKwTix7JyAnfVxuICAgICAgICAgICAgICAgICAge3NlbGVjdGVkQ2VudGVyVW5pdC5sb25naXR1ZGUudG9GaXhlZCg1KX3CsEVcbiAgICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICAgICAgVOG7jWEgxJHhu5kgVk4tMjAwMDogWD17c2VsZWN0ZWRDZW50ZXJVbml0Lnhfdm4yMDAwLnRvTG9jYWxlU3RyaW5nKCl9bSwgWT1cbiAgICAgICAgICAgICAgICAgIHtzZWxlY3RlZENlbnRlclVuaXQueV92bjIwMDAudG9Mb2NhbGVTdHJpbmcoKX1tXG4gICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgKX1cbiAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgIHsvKiBSZXN1bHRzIHdpdGhpbiBCdWZmZXIgKi99XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS04MDAgcm91bmRlZC14bCBwLTUgc2hhZG93LW1kIHNwYWNlLXktNFwiPlxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gYm9yZGVyLWIgYm9yZGVyLXNsYXRlLTgwMCBwYi0zXCI+XG4gICAgICAgICAgICAgIDxoMyBjbGFzc05hbWU9XCJ0ZXh0LXNtIGZvbnQtYm9sZCB0ZXh0LXdoaXRlIHVwcGVyY2FzZSB0cmFja2luZy13aWRlciBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMlwiPlxuICAgICAgICAgICAgICAgIDxzcGFuPlxuICAgICAgICAgICAgICAgICAgxJDGoW4gduG7iyBu4bqxbSB0cm9uZyB2w7luZyDEkeG7h20gUiA9IHtidWZmZXJSYWRpdXNLbX0ga20gKHt1bml0c1dpdGhpbkJ1ZmZlci5sZW5ndGh9IMSRxqFuIHbhu4spXG4gICAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICA8L2gzPlxuICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXhzIHRleHQtc2xhdGUtNDAwIGZvbnQtbW9ub1wiPlxuICAgICAgICAgICAgICAgIERp4buHbiB0w61jaCB2w7luZyDEkeG7h206IHtNYXRoLnJvdW5kKE1hdGguUEkgKiBidWZmZXJSYWRpdXNLbSAqIGJ1ZmZlclJhZGl1c0ttKX0ga23CslxuICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAge3VuaXRzV2l0aGluQnVmZmVyLmxlbmd0aCA9PT0gMCA/IChcbiAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJwLTggdGV4dC1jZW50ZXIgdGV4dC1zbGF0ZS00MDAgdGV4dC14c1wiPlxuICAgICAgICAgICAgICAgIEtow7RuZyBjw7MgxJHGoW4gduG7iyBuw6BvIGtow6FjIG7hurFtIHRyb25nIHBo4bqhbSB2aSBiw6FuIGvDrW5oIG7DoHkuIEjDo3kgdMSDbmcgYsOhbiBrw61uaCB2w7luZyDEkeG7h20uXG4gICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgKSA6IChcbiAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJncmlkIGdyaWQtY29scy0xIG1kOmdyaWQtY29scy0yIGxnOmdyaWQtY29scy0zIGdhcC0zXCI+XG4gICAgICAgICAgICAgICAge3VuaXRzV2l0aGluQnVmZmVyLm1hcCgoeyB1bml0LCBkaXN0YW5jZUttIH0pID0+IHtcbiAgICAgICAgICAgICAgICAgIC8vIFJvYWQgbW9iaWxpdHkgdGltZSBhdCA0MGttL2hcbiAgICAgICAgICAgICAgICAgIGNvbnN0IHJvYWRFc3RNaW51dGVzID0gTWF0aC5yb3VuZCgoZGlzdGFuY2VLbSAqIDEuMzUgKiA2MCkgLyA0MCk7XG5cbiAgICAgICAgICAgICAgICAgIHJldHVybiAoXG4gICAgICAgICAgICAgICAgICAgIDxkaXZcbiAgICAgICAgICAgICAgICAgICAgICBrZXk9e3VuaXQuaWR9XG4gICAgICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwicC0zIGJnLVsjMGExMTIwXSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCByb3VuZGVkLWxnIGhvdmVyOmJvcmRlci1hbWJlci01MDAgdHJhbnNpdGlvbiBncm91cCBmbGV4IGZsZXgtY29sIGp1c3RpZnktYmV0d2VlblwiXG4gICAgICAgICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gbWItMS41XCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIGZvbnQtbW9ubyBweC0xLjUgcHktMC41IHJvdW5kZWQgYmctc2xhdGUtODAwIHRleHQtYW1iZXItMzAwIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIGZvbnQtYm9sZFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIHt1bml0LmNvZGV9XG4gICAgICAgICAgICAgICAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC14cyBmb250LW1vbm8gZm9udC1ib2xkIHRleHQtYW1iZXItNDAwXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgQ+G7sSBseToge2Rpc3RhbmNlS219IGttXG4gICAgICAgICAgICAgICAgICAgICAgICAgIDwvc3Bhbj5cbiAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgICAgICAgPGg0IGNsYXNzTmFtZT1cInRleHQteHMgZm9udC1ib2xkIHRleHQtd2hpdGUgZ3JvdXAtaG92ZXI6dGV4dC1hbWJlci0zMDAgbGluZS1jbGFtcC0xXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgIHt1bml0Lm5hbWV9XG4gICAgICAgICAgICAgICAgICAgICAgICA8L2g0PlxuICAgICAgICAgICAgICAgICAgICAgICAgPHAgY2xhc3NOYW1lPVwidGV4dC1bMTFweF0gdGV4dC1zbGF0ZS00MDAgbXQtMSBsaW5lLWNsYW1wLTFcIj57dW5pdC5hZGRyZXNzfTwvcD5cbiAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwibXQtMyBwdC0yIGJvcmRlci10IGJvcmRlci1zbGF0ZS04MDAvODAgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuIHRleHQtWzExcHhdXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMSB0ZXh0LXNsYXRlLTQwMFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICA8Q2xvY2sgY2xhc3NOYW1lPVwidy0zIGgtMyB0ZXh0LWVtZXJhbGQtNDAwXCIgLz5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgPHNwYW4+Q8ahIMSR4buZbmcgfntyb2FkRXN0TWludXRlc30gcGjDunQ8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICAgICAgICAgICAgICAgIG9uQ2xpY2s9eygpID0+IG9uU2VsZWN0VW5pdEFuZEdvVG9NYXAodW5pdCl9XG4gICAgICAgICAgICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cInRleHQtZW1lcmFsZC00MDAgaG92ZXI6dW5kZXJsaW5lIGZvbnQtc2VtaWJvbGRcIlxuICAgICAgICAgICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgICAgICAgICBYZW0gduG7iyB0csOtIOKGklxuICAgICAgICAgICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgKTtcbiAgICAgICAgICAgICAgICB9KX1cbiAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICApfVxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cbiAgICAgICl9XG5cbiAgICAgIHsvKiAyLiBLTk4gTkVBUkVTVCBORUlHSEJPUlMgKi99XG4gICAgICB7YWN0aXZlQW5hbHlzaXMgPT09ICdrbm4nICYmIChcbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJzcGFjZS15LTZcIj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImJnLVsjMGUxNjI4XSBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCByb3VuZGVkLXhsIHAtNSBzaGFkb3ctbWRcIj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC14cyBmb250LWJvbGQgdXBwZXJjYXNlIHRleHQtc2t5LTQwMCBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMiBtYi0zXCI+XG4gICAgICAgICAgICAgIDxDb21wYXNzIGNsYXNzTmFtZT1cInctNCBoLTRcIiAvPlxuICAgICAgICAgICAgICA8c3Bhbj5Uw4xNIEtJ4bq+TSDEkMagTiBW4buKIEfhuqZOIE5I4bqkVCAoSy1ORUFSRVNUIE5FSUdIQk9SUyBRVUVSWSk8L3NwYW4+XG4gICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJncmlkIGdyaWQtY29scy0xIG1kOmdyaWQtY29scy0zIGdhcC00XCI+XG4gICAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgICAgPGxhYmVsIGNsYXNzTmFtZT1cInRleHQtWzEwcHhdIHRleHQtc2xhdGUtNDAwIHVwcGVyY2FzZSBmb250LWJvbGQgYmxvY2sgbWItMVwiPlxuICAgICAgICAgICAgICAgICAgxJDGoW4gduG7iyAvIFbhu4sgdHLDrSB5w6p1IGPhuqd1IGNoaSB2aeG7h246XG4gICAgICAgICAgICAgICAgPC9sYWJlbD5cbiAgICAgICAgICAgICAgICA8c2VsZWN0XG4gICAgICAgICAgICAgICAgICB2YWx1ZT17a25uVGFyZ2V0VW5pdElkfVxuICAgICAgICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PiBzZXRLbm5UYXJnZXRVbml0SWQoZS50YXJnZXQudmFsdWUpfVxuICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwidy1mdWxsIGJnLVsjMDkwZjFkXSBib3JkZXIgYm9yZGVyLXNsYXRlLTcwMCByb3VuZGVkLWxnIHB4LTMgcHktMiB0ZXh0LXhzIHRleHQtd2hpdGUgZm9jdXM6b3V0bGluZS1ub25lIGZvY3VzOmJvcmRlci1za3ktNTAwXCJcbiAgICAgICAgICAgICAgICA+XG4gICAgICAgICAgICAgICAgICB7dW5pdHMubWFwKCh1KSA9PiAoXG4gICAgICAgICAgICAgICAgICAgIDxvcHRpb24ga2V5PXt1LmlkfSB2YWx1ZT17dS5pZH0+XG4gICAgICAgICAgICAgICAgICAgICAgW3t1LmNvZGV9XSB7dS5uYW1lfVxuICAgICAgICAgICAgICAgICAgICA8L29wdGlvbj5cbiAgICAgICAgICAgICAgICAgICkpfVxuICAgICAgICAgICAgICAgIDwvc2VsZWN0PlxuICAgICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICAgIDxsYWJlbCBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIGJsb2NrIG1iLTFcIj5cbiAgICAgICAgICAgICAgICAgIFPhu5EgbMaw4bujbmcgxJHGoW4gduG7iyBn4bqnbiBuaOG6pXQgKEspOlxuICAgICAgICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgICAgICAgICAgPHNlbGVjdFxuICAgICAgICAgICAgICAgICAgdmFsdWU9e2tubkNvdW50fVxuICAgICAgICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PiBzZXRLbm5Db3VudChOdW1iZXIoZS50YXJnZXQudmFsdWUpKX1cbiAgICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cInctZnVsbCBiZy1bIzA5MGYxZF0gYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZC1sZyBweC0zIHB5LTIgdGV4dC14cyB0ZXh0LXdoaXRlIGZvbnQtbW9ubyBmb2N1czpvdXRsaW5lLW5vbmUgZm9jdXM6Ym9yZGVyLXNreS01MDBcIlxuICAgICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9ezN9PlRvcCAzIMSRxqFuIHbhu4sgZ+G6p24gbmjhuqV0PC9vcHRpb24+XG4gICAgICAgICAgICAgICAgICA8b3B0aW9uIHZhbHVlPXs1fT5Ub3AgNSDEkcahbiB24buLIGfhuqduIG5o4bqldDwvb3B0aW9uPlxuICAgICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT17OH0+VG9wIDggxJHGoW4gduG7iyBn4bqnbiBuaOG6pXQ8L29wdGlvbj5cbiAgICAgICAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9ezEyfT5Ub3AgMTIgxJHGoW4gduG7iyBn4bqnbiBuaOG6pXQ8L29wdGlvbj5cbiAgICAgICAgICAgICAgICA8L3NlbGVjdD5cbiAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWVuZFwiPlxuICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwicC0yLjUgcm91bmRlZCBiZy1za3ktOTUwLzQwIGJvcmRlciBib3JkZXItc2t5LTgwMCB0ZXh0LVsxMXB4XSB0ZXh0LXNreS0zMDAgdy1mdWxsXCI+XG4gICAgICAgICAgICAgICAgICBQaMO5IGjhu6NwIGNobyB0w6xuaCBodeG7kW5nIGPhu6l1IGjhu5kgY+G7qXUgbuG6oW4ga2jhuqluIGPhuqVwLCB0w6FjIGNoaeG6v24gQTIsIGhv4bq3YyDEkWnhu4F1IMSR4buZbmcgaOG7j2FcbiAgICAgICAgICAgICAgICAgIGzhu7FjIGNoaSB2aeG7h24gbmhhbmggbmjhuqV0LlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgey8qIFJlc3VsdHMgVGFibGUgKi99XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS04MDAgcm91bmRlZC14bCBvdmVyZmxvdy1oaWRkZW4gc2hhZG93LW1kXCI+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInAtNCBib3JkZXItYiBib3JkZXItc2xhdGUtODAwIGZsZXgganVzdGlmeS1iZXR3ZWVuIGl0ZW1zLWNlbnRlclwiPlxuICAgICAgICAgICAgICA8aDMgY2xhc3NOYW1lPVwidGV4dC1zbSBmb250LWJvbGQgdGV4dC13aGl0ZSB1cHBlcmNhc2UgdHJhY2tpbmctd2lkZXJcIj5cbiAgICAgICAgICAgICAgICBL4bq/dCBxdeG6oyB7a25uQ291bnR9IMSRxqFuIHbhu4sgZ+G6p24gbmjhuqV0IHNvIHbhu5tpIFwie2tublRhcmdldFVuaXQ/Lm5hbWV9XCJcbiAgICAgICAgICAgICAgPC9oMz5cbiAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC14cyB0ZXh0LWVtZXJhbGQtNDAwIGZvbnQtbW9ub1wiPlxuICAgICAgICAgICAgICAgIFTDrW5oIHRoZW8gdGh14bqtdCB0b8OhbiBraG/huqNuZyBjw6FjaCBIYXZlcnNpbmUgJiBjaGnhur91IFVUTVxuICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICA8L2Rpdj5cblxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJvdmVyZmxvdy14LWF1dG9cIj5cbiAgICAgICAgICAgICAgPHRhYmxlIGNsYXNzTmFtZT1cInctZnVsbCB0ZXh0LWxlZnQgdGV4dC14cyB0ZXh0LXNsYXRlLTMwMFwiPlxuICAgICAgICAgICAgICAgIDx0aGVhZCBjbGFzc05hbWU9XCJiZy1bIzA5MGYxZF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIHRleHQtWzEwcHhdIHRyYWNraW5nLXdpZGVyIGZvbnQtbW9ubyBib3JkZXItYiBib3JkZXItc2xhdGUtODAwXCI+XG4gICAgICAgICAgICAgICAgICA8dHI+XG4gICAgICAgICAgICAgICAgICAgIDx0aCBjbGFzc05hbWU9XCJwLTNcIj5UaOG7qSB04buxPC90aD5cbiAgICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtM1wiPk3DoyDEkcahbiB24buLPC90aD5cbiAgICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtM1wiPlTDqm4gxJHGoW4gduG7iyBjaGkgdmnhu4duPC90aD5cbiAgICAgICAgICAgICAgICAgICAgPHRoIGNsYXNzTmFtZT1cInAtM1wiPkJpbmggY2jhu6duZzwvdGg+XG4gICAgICAgICAgICAgICAgICAgIDx0aCBjbGFzc05hbWU9XCJwLTNcIj5LaG/huqNuZyBjw6FjaCDEkcaw4budbmcgdGjhurNuZzwvdGg+XG4gICAgICAgICAgICAgICAgICAgIDx0aCBjbGFzc05hbWU9XCJwLTNcIj7Gr+G7m2MgdMOtbmggdGjhu51pIGdpYW4gY8ahIMSR4buZbmc8L3RoPlxuICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0zXCI+VHLhu7FjIGJhbiB0w6FjIGNoaeG6v248L3RoPlxuICAgICAgICAgICAgICAgICAgICA8dGggY2xhc3NOYW1lPVwicC0zIHRleHQtcmlnaHRcIj5YZW0gR0lTPC90aD5cbiAgICAgICAgICAgICAgICAgIDwvdHI+XG4gICAgICAgICAgICAgICAgPC90aGVhZD5cbiAgICAgICAgICAgICAgICA8dGJvZHkgY2xhc3NOYW1lPVwiZGl2aWRlLXkgZGl2aWRlLXNsYXRlLTgwMFwiPlxuICAgICAgICAgICAgICAgICAge25lYXJlc3RSZXN1bHRzLm1hcCgoeyB1bml0LCBkaXN0YW5jZUttIH0sIGlkeCkgPT4ge1xuICAgICAgICAgICAgICAgICAgICBjb25zdCByb2FkRXN0TWludXRlcyA9IE1hdGgucm91bmQoKGRpc3RhbmNlS20gKiAxLjM1ICogNjApIC8gNDApO1xuICAgICAgICAgICAgICAgICAgICByZXR1cm4gKFxuICAgICAgICAgICAgICAgICAgICAgIDx0ciBrZXk9e3VuaXQuaWR9IGNsYXNzTmFtZT1cImhvdmVyOmJnLXNsYXRlLTgwMC82MCB0cmFuc2l0aW9uXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0zIGZvbnQtbW9ubyBmb250LWJvbGQgdGV4dC1za3ktNDAwXCI+I3tpZHggKyAxfTwvdGQ+XG4gICAgICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0zIGZvbnQtbW9ubyB0ZXh0LWVtZXJhbGQtNDAwIGZvbnQtc2VtaWJvbGRcIj57dW5pdC5jb2RlfTwvdGQ+XG4gICAgICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0zIGZvbnQtYm9sZCB0ZXh0LXdoaXRlXCI+e3VuaXQubmFtZX08L3RkPlxuICAgICAgICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtM1wiPnt1bml0LmJyYW5jaH08L3RkPlxuICAgICAgICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtMyBmb250LW1vbm8gdGV4dC1hbWJlci0zMDAgZm9udC1ib2xkXCI+XG4gICAgICAgICAgICAgICAgICAgICAgICAgIHtkaXN0YW5jZUttfSBrbSAoe01hdGgucm91bmQoZGlzdGFuY2VLbSAqIDEwMDApLnRvTG9jYWxlU3RyaW5nKCl9IG0pXG4gICAgICAgICAgICAgICAgICAgICAgICA8L3RkPlxuICAgICAgICAgICAgICAgICAgICAgICAgPHRkIGNsYXNzTmFtZT1cInAtMyBmb250LW1vbm8gdGV4dC1lbWVyYWxkLTMwMFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICB+e3JvYWRFc3RNaW51dGVzfSBwaMO6dCAoNDAga20vaClcbiAgICAgICAgICAgICAgICAgICAgICAgIDwvdGQ+XG4gICAgICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0zIGZvbnQtbW9ub1wiPnt1bml0LnBob25lfTwvdGQ+XG4gICAgICAgICAgICAgICAgICAgICAgICA8dGQgY2xhc3NOYW1lPVwicC0zIHRleHQtcmlnaHRcIj5cbiAgICAgICAgICAgICAgICAgICAgICAgICAgPGJ1dHRvblxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIG9uQ2xpY2s9eygpID0+IG9uU2VsZWN0VW5pdEFuZEdvVG9NYXAodW5pdCl9XG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwiYmctc2t5LTcwMC84MCBob3ZlcjpiZy1za3ktNjAwIHRleHQtd2hpdGUgcHgtMi41IHB5LTEgcm91bmRlZCB0ZXh0LVsxMXB4XSBmb250LXNlbWlib2xkXCJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIMSQ4buLbmggduG7i1xuICAgICAgICAgICAgICAgICAgICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgICAgICAgICAgICAgICAgIDwvdGQ+XG4gICAgICAgICAgICAgICAgICAgICAgPC90cj5cbiAgICAgICAgICAgICAgICAgICAgKTtcbiAgICAgICAgICAgICAgICAgIH0pfVxuICAgICAgICAgICAgICAgIDwvdGJvZHk+XG4gICAgICAgICAgICAgIDwvdGFibGU+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9kaXY+XG4gICAgICApfVxuXG4gICAgICB7LyogMy4gREVGRU5TSVZFIFNFQ1RPUlMgKi99XG4gICAgICB7YWN0aXZlQW5hbHlzaXMgPT09ICdzZWN0b3JzJyAmJiAoXG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwic3BhY2UteS02XCI+XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJwLTQgcm91bmRlZC14bCBiZy1bIzBlMTYyOF0gYm9yZGVyIGJvcmRlci1zbGF0ZS04MDAgdGV4dC14cyB0ZXh0LXNsYXRlLTMwMCBzcGFjZS15LTJcIj5cbiAgICAgICAgICAgIDxoMyBjbGFzc05hbWU9XCJ0ZXh0LXNtIGZvbnQtYm9sZCB0ZXh0LXdoaXRlIHVwcGVyY2FzZSBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMlwiPlxuICAgICAgICAgICAgICA8U2hpZWxkIGNsYXNzTmFtZT1cInctNCBoLTQgdGV4dC1lbWVyYWxkLTQwMFwiIC8+XG4gICAgICAgICAgICAgIDxzcGFuPjUgS2h1IHbhu7FjIFBow7JuZyB0aOG7pyBDaGnhur9uIGzGsOG7o2MgY+G7p2EgVGjDoG5oIHBo4buRIEjDoCBO4buZaTwvc3Bhbj5cbiAgICAgICAgICAgIDwvaDM+XG4gICAgICAgICAgICA8cD5cbiAgICAgICAgICAgICAgQuG7kSB0csOtIGtow7RuZyBnaWFuIHBow7JuZyB0aOG7pyBwaMOibiBi4buVIHRoZW8gNSBoxrDhu5tuZyB0aGVuIGNo4buRdCBuaOG6sW0gbmfEg24gY2jhurduIG3hu41pIGjGsOG7m25nIHRp4bq/blxuICAgICAgICAgICAgICBjw7RuZywgYuG6o28gduG7hyBhbiBuaW5oIHRy4bqtdCB04buxIHbDoCBz4bq1biBzw6BuZyBjxqEgxJHhu5luZyBwaMOybmcgY2jhu5FuZyB0aGnDqm4gdGFpIGzhu6V0IGLDo28uXG4gICAgICAgICAgICA8L3A+XG4gICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImdyaWQgZ3JpZC1jb2xzLTEgbWQ6Z3JpZC1jb2xzLTIgbGc6Z3JpZC1jb2xzLTMgZ2FwLTRcIj5cbiAgICAgICAgICAgIHtPYmplY3QuZW50cmllcyhzZWN0b3JCcmVha2Rvd24pLm1hcCgoW3NlY3Rvck5hbWUsIGRhdGFdKSA9PiAoXG4gICAgICAgICAgICAgIDxkaXZcbiAgICAgICAgICAgICAgICBrZXk9e3NlY3Rvck5hbWV9XG4gICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwiYmctWyMwZTE2MjhdIGJvcmRlciBib3JkZXItc2xhdGUtODAwIHJvdW5kZWQteGwgcC00IHNoYWRvdy1tZCBzcGFjZS15LTNcIlxuICAgICAgICAgICAgICA+XG4gICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWJldHdlZW4gYm9yZGVyLWIgYm9yZGVyLXNsYXRlLTgwMCBwYi0yXCI+XG4gICAgICAgICAgICAgICAgICA8aDQgY2xhc3NOYW1lPVwidGV4dC14cyBmb250LWJvbGQgdGV4dC13aGl0ZSB1cHBlcmNhc2VcIj57c2VjdG9yTmFtZX08L2g0PlxuICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC14cyBmb250LW1vbm8gcHgtMiBweS0wLjUgcm91bmRlZCBiZy1lbWVyYWxkLTk1MCB0ZXh0LWVtZXJhbGQtMzAwIGJvcmRlciBib3JkZXItZW1lcmFsZC04MDAgZm9udC1ib2xkXCI+XG4gICAgICAgICAgICAgICAgICAgIHtkYXRhLmNvdW50fSDEkcahbiB24buLXG4gICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTQwMCB1cHBlcmNhc2UgZm9udC1ib2xkIG1iLTFcIj5cbiAgICAgICAgICAgICAgICAgICAgQ8OhYyBxdeG6rW4vaHV54buHbiB0cuG7sWMgdGh14buZYyAoe2RhdGEuZGlzdHJpY3RzLmxlbmd0aH0pOlxuICAgICAgICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggZmxleC13cmFwIGdhcC0xXCI+XG4gICAgICAgICAgICAgICAgICAgIHtkYXRhLmRpc3RyaWN0cy5tYXAoKGQpID0+IChcbiAgICAgICAgICAgICAgICAgICAgICA8c3BhblxuICAgICAgICAgICAgICAgICAgICAgICAga2V5PXtkfVxuICAgICAgICAgICAgICAgICAgICAgICAgY2xhc3NOYW1lPVwicHgtMS41IHB5LTAuNSByb3VuZGVkIGJnLXNsYXRlLTkwMCBib3JkZXIgYm9yZGVyLXNsYXRlLTgwMCB0ZXh0LVsxMHB4XSB0ZXh0LXNsYXRlLTMwMFwiXG4gICAgICAgICAgICAgICAgICAgICAgPlxuICAgICAgICAgICAgICAgICAgICAgICAge2R9XG4gICAgICAgICAgICAgICAgICAgICAgPC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICApKX1cbiAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJwdC0yIGJvcmRlci10IGJvcmRlci1zbGF0ZS04MDAvODBcIj5cbiAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1zbGF0ZS00MDAgdXBwZXJjYXNlIGZvbnQtYm9sZCBtYi0xXCI+XG4gICAgICAgICAgICAgICAgICAgIE3hu5l0IHPhu5EgxJHGoW4gduG7iyBuw7JuZyBj4buRdDpcbiAgICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICAgICAgPHVsIGNsYXNzTmFtZT1cInRleHQteHMgdGV4dC1zbGF0ZS0zMDAgc3BhY2UteS0xXCI+XG4gICAgICAgICAgICAgICAgICAgIHtkYXRhLnVuaXRzLnNsaWNlKDAsIDMpLm1hcCgodSkgPT4gKFxuICAgICAgICAgICAgICAgICAgICAgIDxsaSBrZXk9e3UuaWR9IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlblwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidHJ1bmNhdGUgcHItMlwiPuKAoiB7dS5zaG9ydE5hbWV9PC9zcGFuPlxuICAgICAgICAgICAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1lbWVyYWxkLTQwMCBmb250LW1vbm8gZmxleC1zaHJpbmstMFwiPlxuICAgICAgICAgICAgICAgICAgICAgICAgICB7dS5icmFuY2h9XG4gICAgICAgICAgICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgICAgICAgICAgPC9saT5cbiAgICAgICAgICAgICAgICAgICAgKSl9XG4gICAgICAgICAgICAgICAgICA8L3VsPlxuICAgICAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgICkpfVxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cbiAgICAgICl9XG4gICAgPC9kaXY+XG4gICk7XG59O1xuIl19
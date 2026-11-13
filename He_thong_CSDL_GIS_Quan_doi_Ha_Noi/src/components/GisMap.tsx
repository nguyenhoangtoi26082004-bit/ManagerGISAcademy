const React = __vite__cjsImport0_react; const useEffect = __vite__cjsImport0_react["useEffect"]; const useRef = __vite__cjsImport0_react["useRef"]; const useState = __vite__cjsImport0_react["useState"];const L = __vite__cjsImport1_leaflet;const _jsxDEV = __vite__cjsImport6_react_jsxDevRuntime["jsxDEV"];import __vite__cjsImport0_react from "/node_modules/.vite/deps/react.js?v=de843665";
import __vite__cjsImport1_leaflet from "/node_modules/.vite/deps/leaflet.js?v=de843665";
import { HANOI_CENTER, HANOI_BOUNDS, HANOI_OUTER_POLYGON, HANOI_DISTRICTS } from "/src/data/hanoiDistricts.ts";
import { wgs84ToUtm48N, calculateDistanceKm } from "/src/utils/coordinateConversion.ts";
import { createBufferPolygon } from "/src/utils/gisCalculations.ts";
import { ZoomIn, ZoomOut, Maximize2, Info, Ruler, Layers, RotateCcw, CircleDot, X } from "/node_modules/.vite/deps/lucide-react.js?v=de843665";
var _jsxFileName = "/app/applet/src/components/GisMap.tsx";
import __vite__cjsImport6_react_jsxDevRuntime from "/node_modules/.vite/deps/react_jsx-dev-runtime.js?v=de843665";
export const GisMap = ({ units, selectedUnit, onSelectUnit, onUpdateCursorCoord, bufferConfig, onSetBufferConfig, filterBranch }) => {
	const mapContainerRef = useRef(null);
	const mapRef = useRef(null);
	// Layer groups refs
	const tileLayerRef = useRef(null);
	const unitsLayerRef = useRef(null);
	const districtsLayerRef = useRef(null);
	const bufferLayerRef = useRef(null);
	const measureLayerRef = useRef(null);
	const hanoiBoundaryLayerRef = useRef(null);
	// GIS states
	const [activeTool, setActiveTool] = useState("pan");
	const [basemap, setBasemap] = useState("satellite");
	const [showDistricts, setShowDistricts] = useState(true);
	const [showUnits, setShowUnits] = useState(true);
	const [showLabels, setShowLabels] = useState(true);
	const [showHanoiBoundary, setShowHanoiBoundary] = useState(true);
	const [showLayerPanel, setShowLayerPanel] = useState(false);
	const [zoomLevel, setZoomLevel] = useState(10);
	const [measurePoints, setMeasurePoints] = useState([]);
	const [measuredDistance, setMeasuredDistance] = useState(null);
	const [selectedDistrict, setSelectedDistrict] = useState(null);
	// Basemap Tile URLs
	const basemapUrls = {
		satellite: {
			url: "https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}",
			attribution: "Esri, Maxar, Earthstar Geographics",
			name: "Vệ tinh Esri"
		},
		dark: {
			url: "https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png",
			attribution: "© CartoDB, © OpenStreetMap",
			name: "Bản đồ Tác chiến (Dark)"
		},
		topo: {
			url: "https://{s}.tile.opentopomap.org/{z}/{x}/{y}.png",
			attribution: "© OpenTopoMap, © OpenStreetMap",
			name: "Địa hình Quân sự (Topo)"
		},
		osm: {
			url: "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
			attribution: "© OpenStreetMap contributors",
			name: "Giao thông OSM"
		}
	};
	// Branch colors and military symbol helper
	const getBranchBadgeStyle = (branch) => {
		switch (branch) {
			case "Phòng không - Không quân": return {
				bg: "#0284c7",
				text: "#ffffff",
				symbol: "✈"
			};
			case "Binh chủng Đặc công": return {
				bg: "#dc2626",
				text: "#ffffff",
				symbol: "⚔"
			};
			case "Binh chủng Pháo binh": return {
				bg: "#ea580c",
				text: "#ffffff",
				symbol: "💥"
			};
			case "Binh chủng Tăng Thiết Giáp": return {
				bg: "#b45309",
				text: "#ffffff",
				symbol: "🛡"
			};
			case "Tác chiến Không gian mạng": return {
				bg: "#7c3aed",
				text: "#ffffff",
				symbol: "⚡"
			};
			case "Quân y": return {
				bg: "#e11d48",
				text: "#ffffff",
				symbol: "+"
			};
			case "Binh chủng Thông tin liên lạc": return {
				bg: "#d97706",
				text: "#ffffff",
				symbol: "📡"
			};
			case "Binh chủng Công binh": return {
				bg: "#0d9488",
				text: "#ffffff",
				symbol: "⚓"
			};
			case "Bộ Tư lệnh Lăng": return {
				bg: "#ca8a04",
				text: "#ffffff",
				symbol: "⭐"
			};
			default: return {
				bg: "#10b981",
				text: "#ffffff",
				symbol: "★"
			};
		}
	};
	// 1. Initialize Map
	useEffect(() => {
		if (!mapContainerRef.current || mapRef.current) return;
		// Create Leaflet map centered at Hanoi
		const map = L.map(mapContainerRef.current, {
			center: HANOI_CENTER,
			zoom: 10,
			zoomControl: false,
			minZoom: 9,
			maxZoom: 18,
			maxBounds: [[20.3, 105], [21.6, 106.3]]
		});
		// Basemap tile
		const currentBasemap = basemapUrls[basemap];
		const tiles = L.tileLayer(currentBasemap.url, {
			attribution: currentBasemap.attribution,
			maxZoom: 18
		}).addTo(map);
		tileLayerRef.current = tiles;
		// Layers groups
		districtsLayerRef.current = L.layerGroup().addTo(map);
		hanoiBoundaryLayerRef.current = null;
		bufferLayerRef.current = L.layerGroup().addTo(map);
		measureLayerRef.current = L.layerGroup().addTo(map);
		unitsLayerRef.current = L.layerGroup().addTo(map);
		// Track cursor for status bar
		map.on("mousemove", (e) => {
			const { lat, lng } = e.latlng;
			const utm = wgs84ToUtm48N(lat, lng);
			onUpdateCursorCoord({
				lat,
				lng,
				x: utm.x,
				y: utm.y
			});
		});
		map.on("mouseout", () => {
			onUpdateCursorCoord(null);
		});
		map.on("zoomend", () => {
			setZoomLevel(map.getZoom());
		});
		// Map click for spatial analysis / measurement
		map.on("click", (e) => {
			const { lat, lng } = e.latlng;
			if (activeTool === "measure_distance") {
				setMeasurePoints((prev) => {
					const next = [...prev, [lat, lng]];
					if (next.length >= 2) {
						let total = 0;
						for (let i = 0; i < next.length - 1; i++) {
							total += calculateDistanceKm(next[i][0], next[i][1], next[i + 1][0], next[i + 1][1]);
						}
						setMeasuredDistance(Math.round(total * 100) / 100);
					}
					return next;
				});
			} else if (activeTool === "buffer") {
				onSetBufferConfig({
					active: true,
					centerLat: lat,
					centerLng: lng,
					radiusKm: bufferConfig.radiusKm || 10,
					targetUnitName: `Tọa độ (${lat.toFixed(4)}, ${lng.toFixed(4)})`
				});
			}
		});
		mapRef.current = map;
		return () => {
			map.remove();
			mapRef.current = null;
		};
	}, []);
	// 2. Change Basemap Tile
	useEffect(() => {
		if (!mapRef.current) return;
		if (tileLayerRef.current) {
			mapRef.current.removeLayer(tileLayerRef.current);
		}
		const current = basemapUrls[basemap];
		const newTiles = L.tileLayer(current.url, {
			attribution: current.attribution,
			maxZoom: 18
		}).addTo(mapRef.current);
		newTiles.bringToBack();
		tileLayerRef.current = newTiles;
	}, [basemap]);
	// 3. Render Hanoi outer boundary and Districts
	useEffect(() => {
		if (!mapRef.current) return;
		// Render outer boundary polygon (Hanoi perimeter)
		if (hanoiBoundaryLayerRef.current) {
			mapRef.current.removeLayer(hanoiBoundaryLayerRef.current);
			hanoiBoundaryLayerRef.current = null;
		}
		if (showHanoiBoundary) {
			const outerPoly = L.polygon(HANOI_OUTER_POLYGON, {
				color: "#fbbf24",
				weight: 2.5,
				opacity: .9,
				fillColor: "#fbbf24",
				fillOpacity: .05,
				dashArray: "5, 5"
			}).addTo(mapRef.current);
			hanoiBoundaryLayerRef.current = outerPoly;
		}
		// Render District polygons
		if (districtsLayerRef.current) {
			districtsLayerRef.current.clearLayers();
			if (showDistricts) {
				HANOI_DISTRICTS.forEach((d) => {
					const poly = L.polygon(d.polygon, {
						color: "#38bdf8",
						weight: 1.2,
						opacity: .6,
						fillColor: "#0284c7",
						fillOpacity: selectedDistrict?.code === d.code ? .35 : .08
					});
					// District label / tooltip
					if (showLabels) {
						poly.bindTooltip(`<div class="text-[11px] font-bold text-sky-300 drop-shadow">${d.shortName}</div>`, {
							permanent: zoomLevel >= 11,
							direction: "center",
							className: "district-tooltip bg-transparent border-0 shadow-none text-center"
						});
					}
					poly.on("click", (e) => {
						L.DomEvent.stopPropagation(e);
						setSelectedDistrict(d);
					});
					poly.addTo(districtsLayerRef.current);
				});
			}
		}
	}, [
		showDistricts,
		showLabels,
		showHanoiBoundary,
		zoomLevel,
		selectedDistrict
	]);
	// 4. Render Military Units Markers
	useEffect(() => {
		if (!mapRef.current || !unitsLayerRef.current) return;
		unitsLayerRef.current.clearLayers();
		if (!showUnits) return;
		// Filter units if branch filter active
		const filteredUnits = filterBranch ? units.filter((u) => u.branch === filterBranch) : units;
		filteredUnits.forEach((unit) => {
			const isSelected = selectedUnit?.id === unit.id;
			const badge = getBranchBadgeStyle(unit.branch);
			// Custom SVG Marker with Military PAVN design
			const customIcon = L.divIcon({
				className: "custom-military-marker",
				html: `
          <div class="relative group cursor-pointer flex flex-col items-center">
            ${isSelected ? `<div class="absolute -inset-2 rounded-full border-2 border-emerald-400 radar-ping"></div>` : ""}
            <div style="background-color: ${badge.bg}; border: 2px solid ${isSelected ? "#ffffff" : "#fbbf24"};" class="w-7 h-7 rounded-full flex items-center justify-center text-white font-bold text-xs shadow-lg transform transition-transform duration-200 group-hover:scale-125">
              <span>${badge.symbol}</span>
            </div>
            ${showLabels ? `<div class="mt-1 px-1.5 py-0.5 rounded bg-slate-900/90 border border-slate-700/80 text-[10px] font-semibold text-slate-200 whitespace-nowrap shadow-md pointer-events-none">
                    ${unit.shortName}
                   </div>` : ""}
          </div>
        `,
				iconSize: [28, 28],
				iconAnchor: [14, 14],
				popupAnchor: [0, -16]
			});
			const marker = L.marker([unit.latitude, unit.longitude], { icon: customIcon });
			// Click to select
			marker.on("click", (e) => {
				L.DomEvent.stopPropagation(e);
				onSelectUnit(unit);
			});
			// Interactive Popup
			marker.bindPopup(`
        <div class="p-1 min-w-[240px]">
          <div class="flex items-center gap-2 border-b border-slate-700 pb-2 mb-2">
            <span class="w-3 h-3 rounded-full" style="background-color: ${badge.bg}"></span>
            <div>
              <div class="text-[11px] font-mono text-emerald-400 font-bold">${unit.code}</div>
              <h4 class="text-sm font-bold text-white leading-tight">${unit.name}</h4>
            </div>
          </div>
          <div class="space-y-1.5 text-xs text-slate-300">
            <div><span class="text-slate-400">Binh chủng:</span> <strong class="text-white">${unit.branch}</strong></div>
            <div><span class="text-slate-400">Quận/Huyện:</span> <strong class="text-white">${unit.district}</strong></div>
            <div><span class="text-slate-400">Trực ban:</span> <span class="font-mono text-emerald-300">${unit.phone}</span></div>
            <div><span class="text-slate-400">Tọa độ VN-2000:</span> <span class="font-mono text-sky-300 text-[11px]">X=${unit.x_vn2000.toLocaleString()}m, Y=${unit.y_vn2000.toLocaleString()}m</span></div>
            <div class="mt-2 pt-2 border-t border-slate-800 flex justify-between items-center">
              <span class="text-[10px] px-1.5 py-0.5 rounded ${unit.readinessStatus.includes("Cấp 1") ? "bg-rose-950 text-rose-300 border border-rose-800" : "bg-emerald-950 text-emerald-300 border border-emerald-800"}">${unit.readinessStatus}</span>
              <span class="text-[10px] text-amber-400">Độ mật: ${unit.securityLevel}</span>
            </div>
          </div>
        </div>
      `);
			marker.addTo(unitsLayerRef.current);
		});
	}, [
		units,
		selectedUnit,
		showUnits,
		showLabels,
		filterBranch
	]);
	// 5. Handle Selected Unit pan/flyTo
	useEffect(() => {
		if (selectedUnit && mapRef.current) {
			mapRef.current.flyTo([selectedUnit.latitude, selectedUnit.longitude], 14, { duration: 1.2 });
		}
	}, [selectedUnit]);
	// 6. Render Buffer Zone Analysis
	useEffect(() => {
		if (!mapRef.current || !bufferLayerRef.current) return;
		bufferLayerRef.current.clearLayers();
		if (bufferConfig.active && bufferConfig.centerLat && bufferConfig.centerLng) {
			const bufferCoords = createBufferPolygon(bufferConfig.centerLat, bufferConfig.centerLng, bufferConfig.radiusKm);
			// Circle Buffer Polygon
			const bufferPoly = L.polygon(bufferCoords, {
				color: "#f59e0b",
				weight: 2,
				dashArray: "6, 6",
				fillColor: "#f59e0b",
				fillOpacity: .15
			});
			// Center crosshair marker
			const centerMarker = L.circleMarker([bufferConfig.centerLat, bufferConfig.centerLng], {
				radius: 6,
				color: "#ef4444",
				fillColor: "#ffffff",
				fillOpacity: 1,
				weight: 2
			});
			bufferPoly.bindTooltip(`Vùng đệm bảo vệ: R = ${bufferConfig.radiusKm} km (${bufferConfig.targetUnitName || ""})`, {
				permanent: true,
				direction: "top"
			});
			bufferPoly.addTo(bufferLayerRef.current);
			centerMarker.addTo(bufferLayerRef.current);
		}
	}, [bufferConfig]);
	// 7. Render Measurement Polyline
	useEffect(() => {
		if (!mapRef.current || !measureLayerRef.current) return;
		measureLayerRef.current.clearLayers();
		if (measurePoints.length > 0) {
			// Draw points
			measurePoints.forEach((pt, idx) => {
				L.circleMarker(pt, {
					radius: 5,
					color: "#38bdf8",
					fillColor: idx === 0 ? "#10b981" : "#f59e0b",
					fillOpacity: 1,
					weight: 2
				}).addTo(measureLayerRef.current);
			});
			// Draw polyline
			if (measurePoints.length >= 2) {
				L.polyline(measurePoints, {
					color: "#38bdf8",
					weight: 3,
					dashArray: "4, 4"
				}).addTo(measureLayerRef.current);
			}
		}
	}, [measurePoints]);
	// Tool actions
	const handleZoomIn = () => mapRef.current?.zoomIn();
	const handleZoomOut = () => mapRef.current?.zoomOut();
	const handleFitExtent = () => {
		mapRef.current?.fitBounds(HANOI_BOUNDS);
	};
	const handleClearMeasurement = () => {
		setMeasurePoints([]);
		setMeasuredDistance(null);
		onSetBufferConfig({
			active: false,
			centerLat: 0,
			centerLng: 0,
			radiusKm: 10
		});
		setSelectedDistrict(null);
		onSelectUnit(null);
	};
	return /* @__PURE__ */ _jsxDEV("div", {
		className: "relative w-full h-full flex-1 overflow-hidden bg-[#0b111e]",
		children: [
			/* @__PURE__ */ _jsxDEV("div", {
				ref: mapContainerRef,
				className: "w-full h-full"
			}, void 0, false, {
				fileName: _jsxFileName,
				lineNumber: 450,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "absolute top-4 left-4 z-[1000] flex flex-col gap-1.5 bg-[#0b1324]/90 backdrop-blur-md p-1.5 rounded-lg border border-slate-700/80 shadow-2xl",
				children: [
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: handleZoomIn,
						title: "Phóng to bản đồ (+)",
						className: "w-8 h-8 rounded bg-slate-800 hover:bg-emerald-600 text-slate-200 hover:text-white flex items-center justify-center transition",
						children: /* @__PURE__ */ _jsxDEV(ZoomIn, { className: "w-4 h-4" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 459,
							columnNumber: 11
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 454,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: handleZoomOut,
						title: "Thu nhỏ bản đồ (-)",
						className: "w-8 h-8 rounded bg-slate-800 hover:bg-emerald-600 text-slate-200 hover:text-white flex items-center justify-center transition",
						children: /* @__PURE__ */ _jsxDEV(ZoomOut, { className: "w-4 h-4" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 466,
							columnNumber: 11
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 461,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: handleFitExtent,
						title: "Xem toàn bộ lãnh thổ Hà Nội (Full Extent)",
						className: "w-8 h-8 rounded bg-slate-800 hover:bg-emerald-600 text-slate-200 hover:text-white flex items-center justify-center transition",
						children: /* @__PURE__ */ _jsxDEV(Maximize2, { className: "w-4 h-4" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 473,
							columnNumber: 11
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 468,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", { className: "w-full h-px bg-slate-700 my-0.5" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 476,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: () => setActiveTool(activeTool === "identify" ? "pan" : "identify"),
						title: "Tra cứu thông tin đối tượng (Identify)",
						className: `w-8 h-8 rounded flex items-center justify-center transition ${activeTool === "identify" ? "bg-purple-600 text-white ring-2 ring-purple-400" : "bg-slate-800 hover:bg-purple-700 text-slate-200"}`,
						children: /* @__PURE__ */ _jsxDEV(Info, { className: "w-4 h-4" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 487,
							columnNumber: 11
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 478,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: () => setActiveTool(activeTool === "measure_distance" ? "pan" : "measure_distance"),
						title: "Đo khoảng cách tác chiến cự ly (Measure Tool)",
						className: `w-8 h-8 rounded flex items-center justify-center transition ${activeTool === "measure_distance" ? "bg-sky-600 text-white ring-2 ring-sky-400" : "bg-slate-800 hover:bg-sky-700 text-slate-200"}`,
						children: /* @__PURE__ */ _jsxDEV(Ruler, { className: "w-4 h-4" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 498,
							columnNumber: 11
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 489,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: () => setActiveTool(activeTool === "buffer" ? "pan" : "buffer"),
						title: "Tạo vùng đệm bảo vệ bán kính (Buffer Zone)",
						className: `w-8 h-8 rounded flex items-center justify-center transition ${activeTool === "buffer" ? "bg-amber-600 text-white ring-2 ring-amber-400" : "bg-slate-800 hover:bg-amber-700 text-slate-200"}`,
						children: /* @__PURE__ */ _jsxDEV(CircleDot, { className: "w-4 h-4" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 509,
							columnNumber: 11
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 500,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: () => setShowLayerPanel(!showLayerPanel),
						title: "Quản lý lớp dữ liệu GIS (Layer Stack)",
						className: `w-8 h-8 rounded flex items-center justify-center transition ${showLayerPanel ? "bg-emerald-600 text-white" : "bg-slate-800 hover:bg-emerald-700 text-slate-200"}`,
						children: /* @__PURE__ */ _jsxDEV(Layers, { className: "w-4 h-4" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 520,
							columnNumber: 11
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 511,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: handleClearMeasurement,
						title: "Làm sạch phép đo và vùng chọn (Clear / Reset)",
						className: "w-8 h-8 rounded bg-slate-800 hover:bg-rose-700 text-slate-200 hover:text-white flex items-center justify-center transition",
						children: /* @__PURE__ */ _jsxDEV(RotateCcw, { className: "w-4 h-4" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 527,
							columnNumber: 11
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 522,
						columnNumber: 9
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 453,
				columnNumber: 7
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "absolute top-72 left-5 z-[1000] hidden sm:flex flex-col items-center bg-[#0b1324]/90 backdrop-blur-md px-1.5 py-3 rounded-full border border-slate-700 shadow-xl gap-2",
				children: [
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: handleZoomIn,
						className: "text-xs font-bold text-slate-300 hover:text-emerald-400",
						children: "+"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 533,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("input", {
						type: "range",
						min: 9,
						max: 17,
						value: zoomLevel,
						onChange: (e) => mapRef.current?.setZoom(Number(e.target.value)),
						className: "w-24 h-1 bg-slate-700 rounded-lg appearance-none cursor-pointer -rotate-90 my-10 accent-emerald-500"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 539,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: handleZoomOut,
						className: "text-xs font-bold text-slate-300 hover:text-emerald-400",
						children: "-"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 547,
						columnNumber: 9
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 532,
				columnNumber: 7
			}, this),
			showLayerPanel && /* @__PURE__ */ _jsxDEV("div", {
				className: "absolute top-4 left-16 z-[1001] w-64 bg-[#09101e]/95 backdrop-blur-md border border-slate-700 rounded-lg p-3 text-white shadow-2xl space-y-3",
				children: [
					/* @__PURE__ */ _jsxDEV("div", {
						className: "flex items-center justify-between border-b border-slate-700 pb-2",
						children: [/* @__PURE__ */ _jsxDEV("h4", {
							className: "text-xs font-bold text-emerald-400 flex items-center gap-1.5",
							children: [/* @__PURE__ */ _jsxDEV(Layers, { className: "w-3.5 h-3.5" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 560,
								columnNumber: 15
							}, this), /* @__PURE__ */ _jsxDEV("span", { children: "QUẢN LÝ LỚP BẢN ĐỒ" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 561,
								columnNumber: 15
							}, this)]
						}, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 559,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("button", {
							onClick: () => setShowLayerPanel(false),
							className: "text-slate-400 hover:text-white",
							children: /* @__PURE__ */ _jsxDEV(X, { className: "w-3.5 h-3.5" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 567,
								columnNumber: 15
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 563,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 558,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "space-y-1",
						children: [/* @__PURE__ */ _jsxDEV("span", {
							className: "text-[11px] font-semibold text-slate-300",
							children: "Bản đồ nền:"
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 573,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("div", {
							className: "grid grid-cols-2 gap-1.5 pt-1",
							children: Object.keys(basemapUrls).map((key) => /* @__PURE__ */ _jsxDEV("button", {
								onClick: () => setBasemap(key),
								className: `text-[10px] p-1.5 rounded text-left border ${basemap === key ? "bg-emerald-900/60 border-emerald-500 text-emerald-200 font-bold" : "bg-slate-800 border-slate-700 text-slate-300 hover:bg-slate-700"}`,
								children: basemapUrls[key].name
							}, key, false, {
								fileName: _jsxFileName,
								lineNumber: 576,
								columnNumber: 17
							}, this))
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 574,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 572,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "space-y-2 border-t border-slate-800 pt-2 text-xs",
						children: [
							/* @__PURE__ */ _jsxDEV("span", {
								className: "text-[11px] font-semibold text-slate-300",
								children: "Lớp chuyên đề:"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 593,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("label", {
								className: "flex items-center justify-between cursor-pointer",
								children: [/* @__PURE__ */ _jsxDEV("span", { children: "Đơn vị quân đội" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 595,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("input", {
									type: "checkbox",
									checked: showUnits,
									onChange: (e) => setShowUnits(e.target.checked),
									className: "accent-emerald-500 rounded"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 596,
									columnNumber: 15
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 594,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("label", {
								className: "flex items-center justify-between cursor-pointer",
								children: [/* @__PURE__ */ _jsxDEV("span", { children: "Ranh giới 30 Quận/Huyện" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 604,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("input", {
									type: "checkbox",
									checked: showDistricts,
									onChange: (e) => setShowDistricts(e.target.checked),
									className: "accent-emerald-500 rounded"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 605,
									columnNumber: 15
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 603,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("label", {
								className: "flex items-center justify-between cursor-pointer",
								children: [/* @__PURE__ */ _jsxDEV("span", { children: "Đường biên TP. Hà Nội" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 613,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("input", {
									type: "checkbox",
									checked: showHanoiBoundary,
									onChange: (e) => setShowHanoiBoundary(e.target.checked),
									className: "accent-emerald-500 rounded"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 614,
									columnNumber: 15
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 612,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("label", {
								className: "flex items-center justify-between cursor-pointer",
								children: [/* @__PURE__ */ _jsxDEV("span", { children: "Nhãn tên đơn vị" }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 622,
									columnNumber: 15
								}, this), /* @__PURE__ */ _jsxDEV("input", {
									type: "checkbox",
									checked: showLabels,
									onChange: (e) => setShowLabels(e.target.checked),
									className: "accent-emerald-500 rounded"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 623,
									columnNumber: 15
								}, this)]
							}, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 621,
								columnNumber: 13
							}, this)
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 592,
						columnNumber: 11
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 557,
				columnNumber: 9
			}, this),
			activeTool === "measure_distance" && /* @__PURE__ */ _jsxDEV("div", {
				className: "absolute top-4 left-1/2 -translate-x-1/2 z-[1000] bg-slate-900/95 border border-sky-500 px-4 py-2 rounded-lg text-xs text-white shadow-xl flex items-center gap-3",
				children: [
					/* @__PURE__ */ _jsxDEV(Ruler, { className: "w-4 h-4 text-sky-400" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 637,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("span", { children: "Bấm chuột trên bản đồ để chọn các điểm đo cự ly tác chiến." }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 638,
						columnNumber: 11
					}, this),
					measuredDistance !== null && /* @__PURE__ */ _jsxDEV("div", {
						className: "bg-sky-950 px-2.5 py-1 rounded border border-sky-400 text-sky-200 font-mono font-bold",
						children: [
							"Tổng cự ly: ",
							measuredDistance,
							" km (",
							Math.round(measuredDistance * 1e3).toLocaleString(),
							" m)"
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 640,
						columnNumber: 13
					}, this),
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: () => {
							setMeasurePoints([]);
							setMeasuredDistance(null);
						},
						className: "text-[11px] text-rose-400 hover:text-rose-300 underline font-medium ml-2",
						children: "Đo lại"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 644,
						columnNumber: 11
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 636,
				columnNumber: 9
			}, this),
			activeTool === "buffer" && /* @__PURE__ */ _jsxDEV("div", {
				className: "absolute top-4 left-1/2 -translate-x-1/2 z-[1000] bg-slate-900/95 border border-amber-500 px-4 py-2 rounded-lg text-xs text-white shadow-xl flex items-center gap-3",
				children: [
					/* @__PURE__ */ _jsxDEV(CircleDot, { className: "w-4 h-4 text-amber-400" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 659,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("span", { children: "Bấm điểm trên bản đồ để tạo vùng đệm phòng thủ. Bán kính:" }, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 660,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("select", {
						value: bufferConfig.radiusKm,
						onChange: (e) => onSetBufferConfig({
							...bufferConfig,
							radiusKm: Number(e.target.value)
						}),
						className: "bg-slate-800 border border-slate-600 rounded px-2 py-1 text-xs text-white font-mono",
						children: [
							/* @__PURE__ */ _jsxDEV("option", {
								value: 3,
								children: "3 km (Phạm vi súng cối/tầm gần)"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 668,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("option", {
								value: 5,
								children: "5 km (Vành đai an ninh trực tiếp)"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 669,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("option", {
								value: 10,
								children: "10 km (Vành đai phòng thủ chiến dịch)"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 670,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("option", {
								value: 20,
								children: "20 km (Vùng hỏa lực pháo binh)"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 671,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("option", {
								value: 35,
								children: "35 km (Tầm tác chiến tên lửa phòng không tầm trung)"
							}, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 672,
								columnNumber: 13
							}, this)
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 661,
						columnNumber: 11
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 658,
				columnNumber: 9
			}, this),
			/* @__PURE__ */ _jsxDEV("div", {
				className: "absolute bottom-6 left-5 z-[1000] bg-[#09101f]/85 backdrop-blur-sm px-2.5 py-1 rounded border border-slate-700/80 text-[11px] font-mono text-slate-300 flex items-center gap-2 shadow-lg",
				children: [
					/* @__PURE__ */ _jsxDEV("span", {
						className: "font-semibold text-slate-400",
						children: "0"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 679,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "w-20 h-1.5 bg-gradient-to-r from-white via-slate-400 to-white border-t border-b border-slate-900 relative",
						children: /* @__PURE__ */ _jsxDEV("div", { className: "absolute left-1/2 top-0 bottom-0 w-0.5 bg-black" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 681,
							columnNumber: 11
						}, this)
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 680,
						columnNumber: 9
					}, this),
					/* @__PURE__ */ _jsxDEV("span", {
						className: "font-bold text-white",
						children: "12 km"
					}, void 0, false, {
						fileName: _jsxFileName,
						lineNumber: 683,
						columnNumber: 9
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 678,
				columnNumber: 7
			}, this),
			selectedDistrict && /* @__PURE__ */ _jsxDEV("div", {
				className: "absolute bottom-6 right-6 z-[1000] w-80 bg-[#09101e]/95 backdrop-blur-md border border-sky-600 rounded-lg p-3 text-white shadow-2xl space-y-2",
				children: [
					/* @__PURE__ */ _jsxDEV("div", {
						className: "flex items-center justify-between border-b border-slate-700 pb-1.5",
						children: [/* @__PURE__ */ _jsxDEV("div", { children: [/* @__PURE__ */ _jsxDEV("span", {
							className: "text-[10px] text-sky-400 font-bold uppercase",
							children: selectedDistrict.kind
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 691,
							columnNumber: 15
						}, this), /* @__PURE__ */ _jsxDEV("h3", {
							className: "text-sm font-bold text-white",
							children: selectedDistrict.name
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 694,
							columnNumber: 15
						}, this)] }, void 0, true, {
							fileName: _jsxFileName,
							lineNumber: 690,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("button", {
							onClick: () => setSelectedDistrict(null),
							className: "text-slate-400 hover:text-white",
							children: /* @__PURE__ */ _jsxDEV(X, { className: "w-4 h-4" }, void 0, false, {
								fileName: _jsxFileName,
								lineNumber: 700,
								columnNumber: 15
							}, this)
						}, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 696,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 689,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("div", {
						className: "text-xs space-y-1 text-slate-300",
						children: [
							/* @__PURE__ */ _jsxDEV("div", { children: [
								/* @__PURE__ */ _jsxDEV("span", {
									className: "text-slate-400",
									children: "Vùng phòng thủ:"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 705,
									columnNumber: 15
								}, this),
								" ",
								/* @__PURE__ */ _jsxDEV("strong", {
									className: "text-emerald-400",
									children: selectedDistrict.defensiveSector
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 706,
									columnNumber: 15
								}, this)
							] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 704,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [
								/* @__PURE__ */ _jsxDEV("span", {
									className: "text-slate-400",
									children: "Diện tích:"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 709,
									columnNumber: 15
								}, this),
								" ",
								/* @__PURE__ */ _jsxDEV("span", {
									className: "font-mono",
									children: [selectedDistrict.areaKm2, " km²"]
								}, void 0, true, {
									fileName: _jsxFileName,
									lineNumber: 710,
									columnNumber: 15
								}, this)
							] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 708,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [
								/* @__PURE__ */ _jsxDEV("span", {
									className: "text-slate-400",
									children: "Ban CHQS:"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 713,
									columnNumber: 15
								}, this),
								" ",
								/* @__PURE__ */ _jsxDEV("span", { children: selectedDistrict.militaryCommandAddress }, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 714,
									columnNumber: 15
								}, this)
							] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 712,
								columnNumber: 13
							}, this),
							/* @__PURE__ */ _jsxDEV("div", { children: [
								/* @__PURE__ */ _jsxDEV("span", {
									className: "text-slate-400",
									children: "Số máy trực ban:"
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 717,
									columnNumber: 15
								}, this),
								" ",
								/* @__PURE__ */ _jsxDEV("span", {
									className: "font-mono text-emerald-300 font-semibold",
									children: selectedDistrict.militaryCommandPhone
								}, void 0, false, {
									fileName: _jsxFileName,
									lineNumber: 718,
									columnNumber: 15
								}, this)
							] }, void 0, true, {
								fileName: _jsxFileName,
								lineNumber: 716,
								columnNumber: 13
							}, this)
						]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 703,
						columnNumber: 11
					}, this),
					/* @__PURE__ */ _jsxDEV("button", {
						onClick: () => {
							onSetBufferConfig({
								active: true,
								centerLat: selectedDistrict.center[0],
								centerLng: selectedDistrict.center[1],
								radiusKm: 10,
								targetUnitName: selectedDistrict.name
							});
						},
						className: "w-full mt-1 bg-sky-700 hover:bg-sky-600 text-white text-[11px] font-semibold py-1.5 rounded flex items-center justify-center gap-1.5",
						children: [/* @__PURE__ */ _jsxDEV(CircleDot, { className: "w-3.5 h-3.5" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 735,
							columnNumber: 13
						}, this), /* @__PURE__ */ _jsxDEV("span", { children: "Tạo vùng đệm phòng thủ quận (10km)" }, void 0, false, {
							fileName: _jsxFileName,
							lineNumber: 736,
							columnNumber: 13
						}, this)]
					}, void 0, true, {
						fileName: _jsxFileName,
						lineNumber: 723,
						columnNumber: 11
					}, this)
				]
			}, void 0, true, {
				fileName: _jsxFileName,
				lineNumber: 688,
				columnNumber: 9
			}, this)
		]
	}, void 0, true, {
		fileName: _jsxFileName,
		lineNumber: 448,
		columnNumber: 5
	}, this);
};

//# sourceMappingURL=data:application/json;base64,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bUJBQW9CLGlCQUFpQjtRQUF3Qjs7Ozs7T0FDNUU7Ozs7O09BQ0wsd0JBQUMsT0FBRDtRQUNFLHdCQUFDLFFBQUQ7U0FBTSxXQUFVO21CQUFpQjtRQUFnQjs7Ozs7UUFBRTtRQUNuRCx3QkFBQyxRQUFEO1NBQU0sV0FBVTttQkFBaEIsQ0FBNkIsaUJBQWlCLFNBQVEsTUFBVTs7Ozs7O09BQzdEOzs7OztPQUNMLHdCQUFDLE9BQUQ7UUFDRSx3QkFBQyxRQUFEO1NBQU0sV0FBVTttQkFBaUI7UUFBZTs7Ozs7UUFBRTtRQUNsRCx3QkFBQyxRQUFELFlBQU8saUJBQWlCLHVCQUE2Qjs7Ozs7T0FDbEQ7Ozs7O09BQ0wsd0JBQUMsT0FBRDtRQUNFLHdCQUFDLFFBQUQ7U0FBTSxXQUFVO21CQUFpQjtRQUFzQjs7Ozs7UUFBRTtRQUN6RCx3QkFBQyxRQUFEO1NBQU0sV0FBVTttQkFDYixpQkFBaUI7UUFDZDs7Ozs7T0FDSDs7Ozs7TUFDRjs7Ozs7O0tBQ0wsd0JBQUMsVUFBRDtNQUNFLGVBQWU7T0FDYixrQkFBa0I7UUFDaEIsUUFBUTtRQUNSLFdBQVcsaUJBQWlCLE9BQU87UUFDbkMsV0FBVyxpQkFBaUIsT0FBTztRQUNuQyxVQUFVO1FBQ1YsZ0JBQWdCLGlCQUFpQjtPQUNuQyxDQUFDO01BQ0g7TUFDQSxXQUFVO2dCQVZaLENBWUUsd0JBQUMsV0FBRCxFQUFXLFdBQVUsY0FBZTs7OztnQkFDcEMsd0JBQUMsUUFBRCxZQUFNLHFDQUF3Qzs7OztjQUN4Qzs7Ozs7O0lBQ0w7Ozs7OztFQUVKOzs7Ozs7QUFFVCIsIm5hbWVzIjpbXSwic291cmNlcyI6WyJHaXNNYXAudHN4Il0sInZlcnNpb24iOjMsInNvdXJjZXNDb250ZW50IjpbImltcG9ydCBSZWFjdCwgeyB1c2VFZmZlY3QsIHVzZVJlZiwgdXNlU3RhdGUgfSBmcm9tICdyZWFjdCc7XG5pbXBvcnQgTCBmcm9tICdsZWFmbGV0JztcbmltcG9ydCB7IE1pbGl0YXJ5VW5pdCwgRGlzdHJpY3RJbmZvLCBNYXBUb29sVHlwZSwgQmFzZW1hcFR5cGUsIEJ1ZmZlckFuYWx5c2lzIH0gZnJvbSAnLi4vdHlwZXMvZ2lzJztcbmltcG9ydCB7IEhBTk9JX0NFTlRFUiwgSEFOT0lfQk9VTkRTLCBIQU5PSV9PVVRFUl9QT0xZR09OLCBIQU5PSV9ESVNUUklDVFMgfSBmcm9tICcuLi9kYXRhL2hhbm9pRGlzdHJpY3RzJztcbmltcG9ydCB7IHdnczg0VG9VdG00OE4sIGNhbGN1bGF0ZURpc3RhbmNlS20gfSBmcm9tICcuLi91dGlscy9jb29yZGluYXRlQ29udmVyc2lvbic7XG5pbXBvcnQgeyBjcmVhdGVCdWZmZXJQb2x5Z29uIH0gZnJvbSAnLi4vdXRpbHMvZ2lzQ2FsY3VsYXRpb25zJztcbmltcG9ydCB7XG4gIFpvb21JbixcbiAgWm9vbU91dCxcbiAgTWF4aW1pemUyLFxuICBJbmZvLFxuICBSdWxlcixcbiAgTGF5ZXJzLFxuICBSb3RhdGVDY3csXG4gIFNoaWVsZCxcbiAgQ2lyY2xlRG90LFxuICBFeWUsXG4gIEV5ZU9mZixcbiAgRmlsdGVyLFxuICBYLFxufSBmcm9tICdsdWNpZGUtcmVhY3QnO1xuXG5pbnRlcmZhY2UgR2lzTWFwUHJvcHMge1xuICB1bml0czogTWlsaXRhcnlVbml0W107XG4gIHNlbGVjdGVkVW5pdDogTWlsaXRhcnlVbml0IHwgbnVsbDtcbiAgb25TZWxlY3RVbml0OiAodW5pdDogTWlsaXRhcnlVbml0IHwgbnVsbCkgPT4gdm9pZDtcbiAgb25VcGRhdGVDdXJzb3JDb29yZDogKGNvb3JkOiB7IGxhdDogbnVtYmVyOyBsbmc6IG51bWJlcjsgeDogbnVtYmVyOyB5OiBudW1iZXIgfSB8IG51bGwpID0+IHZvaWQ7XG4gIGJ1ZmZlckNvbmZpZzogQnVmZmVyQW5hbHlzaXM7XG4gIG9uU2V0QnVmZmVyQ29uZmlnOiAoY29uZmlnOiBCdWZmZXJBbmFseXNpcykgPT4gdm9pZDtcbiAgZmlsdGVyQnJhbmNoPzogc3RyaW5nO1xufVxuXG5leHBvcnQgY29uc3QgR2lzTWFwOiBSZWFjdC5GQzxHaXNNYXBQcm9wcz4gPSAoe1xuICB1bml0cyxcbiAgc2VsZWN0ZWRVbml0LFxuICBvblNlbGVjdFVuaXQsXG4gIG9uVXBkYXRlQ3Vyc29yQ29vcmQsXG4gIGJ1ZmZlckNvbmZpZyxcbiAgb25TZXRCdWZmZXJDb25maWcsXG4gIGZpbHRlckJyYW5jaCxcbn0pID0+IHtcbiAgY29uc3QgbWFwQ29udGFpbmVyUmVmID0gdXNlUmVmPEhUTUxEaXZFbGVtZW50PihudWxsKTtcbiAgY29uc3QgbWFwUmVmID0gdXNlUmVmPEwuTWFwIHwgbnVsbD4obnVsbCk7XG5cbiAgLy8gTGF5ZXIgZ3JvdXBzIHJlZnNcbiAgY29uc3QgdGlsZUxheWVyUmVmID0gdXNlUmVmPEwuVGlsZUxheWVyIHwgbnVsbD4obnVsbCk7XG4gIGNvbnN0IHVuaXRzTGF5ZXJSZWYgPSB1c2VSZWY8TC5MYXllckdyb3VwIHwgbnVsbD4obnVsbCk7XG4gIGNvbnN0IGRpc3RyaWN0c0xheWVyUmVmID0gdXNlUmVmPEwuR2VvSlNPTiB8IEwuTGF5ZXJHcm91cCB8IG51bGw+KG51bGwpO1xuICBjb25zdCBidWZmZXJMYXllclJlZiA9IHVzZVJlZjxMLkxheWVyR3JvdXAgfCBudWxsPihudWxsKTtcbiAgY29uc3QgbWVhc3VyZUxheWVyUmVmID0gdXNlUmVmPEwuTGF5ZXJHcm91cCB8IG51bGw+KG51bGwpO1xuICBjb25zdCBoYW5vaUJvdW5kYXJ5TGF5ZXJSZWYgPSB1c2VSZWY8TC5Qb2x5Z29uIHwgbnVsbD4obnVsbCk7XG5cbiAgLy8gR0lTIHN0YXRlc1xuICBjb25zdCBbYWN0aXZlVG9vbCwgc2V0QWN0aXZlVG9vbF0gPSB1c2VTdGF0ZTxNYXBUb29sVHlwZT4oJ3BhbicpO1xuICBjb25zdCBbYmFzZW1hcCwgc2V0QmFzZW1hcF0gPSB1c2VTdGF0ZTxCYXNlbWFwVHlwZT4oJ3NhdGVsbGl0ZScpO1xuICBjb25zdCBbc2hvd0Rpc3RyaWN0cywgc2V0U2hvd0Rpc3RyaWN0c10gPSB1c2VTdGF0ZSh0cnVlKTtcbiAgY29uc3QgW3Nob3dVbml0cywgc2V0U2hvd1VuaXRzXSA9IHVzZVN0YXRlKHRydWUpO1xuICBjb25zdCBbc2hvd0xhYmVscywgc2V0U2hvd0xhYmVsc10gPSB1c2VTdGF0ZSh0cnVlKTtcbiAgY29uc3QgW3Nob3dIYW5vaUJvdW5kYXJ5LCBzZXRTaG93SGFub2lCb3VuZGFyeV0gPSB1c2VTdGF0ZSh0cnVlKTtcbiAgY29uc3QgW3Nob3dMYXllclBhbmVsLCBzZXRTaG93TGF5ZXJQYW5lbF0gPSB1c2VTdGF0ZShmYWxzZSk7XG4gIGNvbnN0IFt6b29tTGV2ZWwsIHNldFpvb21MZXZlbF0gPSB1c2VTdGF0ZSgxMCk7XG4gIGNvbnN0IFttZWFzdXJlUG9pbnRzLCBzZXRNZWFzdXJlUG9pbnRzXSA9IHVzZVN0YXRlPFtudW1iZXIsIG51bWJlcl1bXT4oW10pO1xuICBjb25zdCBbbWVhc3VyZWREaXN0YW5jZSwgc2V0TWVhc3VyZWREaXN0YW5jZV0gPSB1c2VTdGF0ZTxudW1iZXIgfCBudWxsPihudWxsKTtcbiAgY29uc3QgW3NlbGVjdGVkRGlzdHJpY3QsIHNldFNlbGVjdGVkRGlzdHJpY3RdID0gdXNlU3RhdGU8RGlzdHJpY3RJbmZvIHwgbnVsbD4obnVsbCk7XG5cbiAgLy8gQmFzZW1hcCBUaWxlIFVSTHNcbiAgY29uc3QgYmFzZW1hcFVybHM6IFJlY29yZDxCYXNlbWFwVHlwZSwgeyB1cmw6IHN0cmluZzsgYXR0cmlidXRpb246IHN0cmluZzsgbmFtZTogc3RyaW5nIH0+ID0ge1xuICAgIHNhdGVsbGl0ZToge1xuICAgICAgdXJsOiAnaHR0cHM6Ly9zZXJ2ZXIuYXJjZ2lzb25saW5lLmNvbS9BcmNHSVMvcmVzdC9zZXJ2aWNlcy9Xb3JsZF9JbWFnZXJ5L01hcFNlcnZlci90aWxlL3t6fS97eX0ve3h9JyxcbiAgICAgIGF0dHJpYnV0aW9uOiAnRXNyaSwgTWF4YXIsIEVhcnRoc3RhciBHZW9ncmFwaGljcycsXG4gICAgICBuYW1lOiAnVuG7hyB0aW5oIEVzcmknLFxuICAgIH0sXG4gICAgZGFyazoge1xuICAgICAgdXJsOiAnaHR0cHM6Ly97c30uYmFzZW1hcHMuY2FydG9jZG4uY29tL2RhcmtfYWxsL3t6fS97eH0ve3l9e3J9LnBuZycsXG4gICAgICBhdHRyaWJ1dGlvbjogJ8KpIENhcnRvREIsIMKpIE9wZW5TdHJlZXRNYXAnLFxuICAgICAgbmFtZTogJ0LhuqNuIMSR4buTIFTDoWMgY2hp4bq/biAoRGFyayknLFxuICAgIH0sXG4gICAgdG9wbzoge1xuICAgICAgdXJsOiAnaHR0cHM6Ly97c30udGlsZS5vcGVudG9wb21hcC5vcmcve3p9L3t4fS97eX0ucG5nJyxcbiAgICAgIGF0dHJpYnV0aW9uOiAnwqkgT3BlblRvcG9NYXAsIMKpIE9wZW5TdHJlZXRNYXAnLFxuICAgICAgbmFtZTogJ8SQ4buLYSBow6xuaCBRdcOibiBz4buxIChUb3BvKScsXG4gICAgfSxcbiAgICBvc206IHtcbiAgICAgIHVybDogJ2h0dHBzOi8ve3N9LnRpbGUub3BlbnN0cmVldG1hcC5vcmcve3p9L3t4fS97eX0ucG5nJyxcbiAgICAgIGF0dHJpYnV0aW9uOiAnwqkgT3BlblN0cmVldE1hcCBjb250cmlidXRvcnMnLFxuICAgICAgbmFtZTogJ0dpYW8gdGjDtG5nIE9TTScsXG4gICAgfSxcbiAgfTtcblxuICAvLyBCcmFuY2ggY29sb3JzIGFuZCBtaWxpdGFyeSBzeW1ib2wgaGVscGVyXG4gIGNvbnN0IGdldEJyYW5jaEJhZGdlU3R5bGUgPSAoYnJhbmNoOiBzdHJpbmcpID0+IHtcbiAgICBzd2l0Y2ggKGJyYW5jaCkge1xuICAgICAgY2FzZSAnUGjDsm5nIGtow7RuZyAtIEtow7RuZyBxdcOibic6XG4gICAgICAgIHJldHVybiB7IGJnOiAnIzAyODRjNycsIHRleHQ6ICcjZmZmZmZmJywgc3ltYm9sOiAn4pyIJyB9O1xuICAgICAgY2FzZSAnQmluaCBjaOG7p25nIMSQ4bq3YyBjw7RuZyc6XG4gICAgICAgIHJldHVybiB7IGJnOiAnI2RjMjYyNicsIHRleHQ6ICcjZmZmZmZmJywgc3ltYm9sOiAn4pqUJyB9O1xuICAgICAgY2FzZSAnQmluaCBjaOG7p25nIFBow6FvIGJpbmgnOlxuICAgICAgICByZXR1cm4geyBiZzogJyNlYTU4MGMnLCB0ZXh0OiAnI2ZmZmZmZicsIHN5bWJvbDogJ/CfkqUnIH07XG4gICAgICBjYXNlICdCaW5oIGNo4bunbmcgVMSDbmcgVGhp4bq/dCBHacOhcCc6XG4gICAgICAgIHJldHVybiB7IGJnOiAnI2I0NTMwOScsIHRleHQ6ICcjZmZmZmZmJywgc3ltYm9sOiAn8J+boScgfTtcbiAgICAgIGNhc2UgJ1TDoWMgY2hp4bq/biBLaMO0bmcgZ2lhbiBt4bqhbmcnOlxuICAgICAgICByZXR1cm4geyBiZzogJyM3YzNhZWQnLCB0ZXh0OiAnI2ZmZmZmZicsIHN5bWJvbDogJ+KaoScgfTtcbiAgICAgIGNhc2UgJ1F1w6JuIHknOlxuICAgICAgICByZXR1cm4geyBiZzogJyNlMTFkNDgnLCB0ZXh0OiAnI2ZmZmZmZicsIHN5bWJvbDogJysnIH07XG4gICAgICBjYXNlICdCaW5oIGNo4bunbmcgVGjDtG5nIHRpbiBsacOqbiBs4bqhYyc6XG4gICAgICAgIHJldHVybiB7IGJnOiAnI2Q5NzcwNicsIHRleHQ6ICcjZmZmZmZmJywgc3ltYm9sOiAn8J+ToScgfTtcbiAgICAgIGNhc2UgJ0JpbmggY2jhu6duZyBDw7RuZyBiaW5oJzpcbiAgICAgICAgcmV0dXJuIHsgYmc6ICcjMGQ5NDg4JywgdGV4dDogJyNmZmZmZmYnLCBzeW1ib2w6ICfimpMnIH07XG4gICAgICBjYXNlICdC4buZIFTGsCBs4buHbmggTMSDbmcnOlxuICAgICAgICByZXR1cm4geyBiZzogJyNjYThhMDQnLCB0ZXh0OiAnI2ZmZmZmZicsIHN5bWJvbDogJ+KtkCcgfTtcbiAgICAgIGRlZmF1bHQ6XG4gICAgICAgIHJldHVybiB7IGJnOiAnIzEwYjk4MScsIHRleHQ6ICcjZmZmZmZmJywgc3ltYm9sOiAn4piFJyB9O1xuICAgIH1cbiAgfTtcblxuICAvLyAxLiBJbml0aWFsaXplIE1hcFxuICB1c2VFZmZlY3QoKCkgPT4ge1xuICAgIGlmICghbWFwQ29udGFpbmVyUmVmLmN1cnJlbnQgfHwgbWFwUmVmLmN1cnJlbnQpIHJldHVybjtcblxuICAgIC8vIENyZWF0ZSBMZWFmbGV0IG1hcCBjZW50ZXJlZCBhdCBIYW5vaVxuICAgIGNvbnN0IG1hcCA9IEwubWFwKG1hcENvbnRhaW5lclJlZi5jdXJyZW50LCB7XG4gICAgICBjZW50ZXI6IEhBTk9JX0NFTlRFUixcbiAgICAgIHpvb206IDEwLFxuICAgICAgem9vbUNvbnRyb2w6IGZhbHNlLCAvLyBDdXN0b20gdG9vbGJhciBoYW5kbGVzIHpvb21cbiAgICAgIG1pblpvb206IDksXG4gICAgICBtYXhab29tOiAxOCxcbiAgICAgIG1heEJvdW5kczogW1xuICAgICAgICBbMjAuMywgMTA1LjBdLFxuICAgICAgICBbMjEuNiwgMTA2LjNdLFxuICAgICAgXSxcbiAgICB9KTtcblxuICAgIC8vIEJhc2VtYXAgdGlsZVxuICAgIGNvbnN0IGN1cnJlbnRCYXNlbWFwID0gYmFzZW1hcFVybHNbYmFzZW1hcF07XG4gICAgY29uc3QgdGlsZXMgPSBMLnRpbGVMYXllcihjdXJyZW50QmFzZW1hcC51cmwsIHtcbiAgICAgIGF0dHJpYnV0aW9uOiBjdXJyZW50QmFzZW1hcC5hdHRyaWJ1dGlvbixcbiAgICAgIG1heFpvb206IDE4LFxuICAgIH0pLmFkZFRvKG1hcCk7XG4gICAgdGlsZUxheWVyUmVmLmN1cnJlbnQgPSB0aWxlcztcblxuICAgIC8vIExheWVycyBncm91cHNcbiAgICBkaXN0cmljdHNMYXllclJlZi5jdXJyZW50ID0gTC5sYXllckdyb3VwKCkuYWRkVG8obWFwKTtcbiAgICBoYW5vaUJvdW5kYXJ5TGF5ZXJSZWYuY3VycmVudCA9IG51bGw7XG4gICAgYnVmZmVyTGF5ZXJSZWYuY3VycmVudCA9IEwubGF5ZXJHcm91cCgpLmFkZFRvKG1hcCk7XG4gICAgbWVhc3VyZUxheWVyUmVmLmN1cnJlbnQgPSBMLmxheWVyR3JvdXAoKS5hZGRUbyhtYXApO1xuICAgIHVuaXRzTGF5ZXJSZWYuY3VycmVudCA9IEwubGF5ZXJHcm91cCgpLmFkZFRvKG1hcCk7XG5cbiAgICAvLyBUcmFjayBjdXJzb3IgZm9yIHN0YXR1cyBiYXJcbiAgICBtYXAub24oJ21vdXNlbW92ZScsIChlOiBMLkxlYWZsZXRNb3VzZUV2ZW50KSA9PiB7XG4gICAgICBjb25zdCB7IGxhdCwgbG5nIH0gPSBlLmxhdGxuZztcbiAgICAgIGNvbnN0IHV0bSA9IHdnczg0VG9VdG00OE4obGF0LCBsbmcpO1xuICAgICAgb25VcGRhdGVDdXJzb3JDb29yZCh7IGxhdCwgbG5nLCB4OiB1dG0ueCwgeTogdXRtLnkgfSk7XG4gICAgfSk7XG5cbiAgICBtYXAub24oJ21vdXNlb3V0JywgKCkgPT4ge1xuICAgICAgb25VcGRhdGVDdXJzb3JDb29yZChudWxsKTtcbiAgICB9KTtcblxuICAgIG1hcC5vbignem9vbWVuZCcsICgpID0+IHtcbiAgICAgIHNldFpvb21MZXZlbChtYXAuZ2V0Wm9vbSgpKTtcbiAgICB9KTtcblxuICAgIC8vIE1hcCBjbGljayBmb3Igc3BhdGlhbCBhbmFseXNpcyAvIG1lYXN1cmVtZW50XG4gICAgbWFwLm9uKCdjbGljaycsIChlOiBMLkxlYWZsZXRNb3VzZUV2ZW50KSA9PiB7XG4gICAgICBjb25zdCB7IGxhdCwgbG5nIH0gPSBlLmxhdGxuZztcblxuICAgICAgaWYgKGFjdGl2ZVRvb2wgPT09ICdtZWFzdXJlX2Rpc3RhbmNlJykge1xuICAgICAgICBzZXRNZWFzdXJlUG9pbnRzKChwcmV2KSA9PiB7XG4gICAgICAgICAgY29uc3QgbmV4dCA9IFsuLi5wcmV2LCBbbGF0LCBsbmddIGFzIFtudW1iZXIsIG51bWJlcl1dO1xuICAgICAgICAgIGlmIChuZXh0Lmxlbmd0aCA+PSAyKSB7XG4gICAgICAgICAgICBsZXQgdG90YWwgPSAwO1xuICAgICAgICAgICAgZm9yIChsZXQgaSA9IDA7IGkgPCBuZXh0Lmxlbmd0aCAtIDE7IGkrKykge1xuICAgICAgICAgICAgICB0b3RhbCArPSBjYWxjdWxhdGVEaXN0YW5jZUttKG5leHRbaV1bMF0sIG5leHRbaV1bMV0sIG5leHRbaSArIDFdWzBdLCBuZXh0W2kgKyAxXVsxXSk7XG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBzZXRNZWFzdXJlZERpc3RhbmNlKE1hdGgucm91bmQodG90YWwgKiAxMDApIC8gMTAwKTtcbiAgICAgICAgICB9XG4gICAgICAgICAgcmV0dXJuIG5leHQ7XG4gICAgICAgIH0pO1xuICAgICAgfSBlbHNlIGlmIChhY3RpdmVUb29sID09PSAnYnVmZmVyJykge1xuICAgICAgICBvblNldEJ1ZmZlckNvbmZpZyh7XG4gICAgICAgICAgYWN0aXZlOiB0cnVlLFxuICAgICAgICAgIGNlbnRlckxhdDogbGF0LFxuICAgICAgICAgIGNlbnRlckxuZzogbG5nLFxuICAgICAgICAgIHJhZGl1c0ttOiBidWZmZXJDb25maWcucmFkaXVzS20gfHwgMTAsXG4gICAgICAgICAgdGFyZ2V0VW5pdE5hbWU6IGBU4buNYSDEkeG7mSAoJHtsYXQudG9GaXhlZCg0KX0sICR7bG5nLnRvRml4ZWQoNCl9KWAsXG4gICAgICAgIH0pO1xuICAgICAgfVxuICAgIH0pO1xuXG4gICAgbWFwUmVmLmN1cnJlbnQgPSBtYXA7XG5cbiAgICByZXR1cm4gKCkgPT4ge1xuICAgICAgbWFwLnJlbW92ZSgpO1xuICAgICAgbWFwUmVmLmN1cnJlbnQgPSBudWxsO1xuICAgIH07XG4gIH0sIFtdKTtcblxuICAvLyAyLiBDaGFuZ2UgQmFzZW1hcCBUaWxlXG4gIHVzZUVmZmVjdCgoKSA9PiB7XG4gICAgaWYgKCFtYXBSZWYuY3VycmVudCkgcmV0dXJuO1xuICAgIGlmICh0aWxlTGF5ZXJSZWYuY3VycmVudCkge1xuICAgICAgbWFwUmVmLmN1cnJlbnQucmVtb3ZlTGF5ZXIodGlsZUxheWVyUmVmLmN1cnJlbnQpO1xuICAgIH1cbiAgICBjb25zdCBjdXJyZW50ID0gYmFzZW1hcFVybHNbYmFzZW1hcF07XG4gICAgY29uc3QgbmV3VGlsZXMgPSBMLnRpbGVMYXllcihjdXJyZW50LnVybCwge1xuICAgICAgYXR0cmlidXRpb246IGN1cnJlbnQuYXR0cmlidXRpb24sXG4gICAgICBtYXhab29tOiAxOCxcbiAgICB9KS5hZGRUbyhtYXBSZWYuY3VycmVudCk7XG4gICAgbmV3VGlsZXMuYnJpbmdUb0JhY2soKTtcbiAgICB0aWxlTGF5ZXJSZWYuY3VycmVudCA9IG5ld1RpbGVzO1xuICB9LCBbYmFzZW1hcF0pO1xuXG4gIC8vIDMuIFJlbmRlciBIYW5vaSBvdXRlciBib3VuZGFyeSBhbmQgRGlzdHJpY3RzXG4gIHVzZUVmZmVjdCgoKSA9PiB7XG4gICAgaWYgKCFtYXBSZWYuY3VycmVudCkgcmV0dXJuO1xuXG4gICAgLy8gUmVuZGVyIG91dGVyIGJvdW5kYXJ5IHBvbHlnb24gKEhhbm9pIHBlcmltZXRlcilcbiAgICBpZiAoaGFub2lCb3VuZGFyeUxheWVyUmVmLmN1cnJlbnQpIHtcbiAgICAgIG1hcFJlZi5jdXJyZW50LnJlbW92ZUxheWVyKGhhbm9pQm91bmRhcnlMYXllclJlZi5jdXJyZW50KTtcbiAgICAgIGhhbm9pQm91bmRhcnlMYXllclJlZi5jdXJyZW50ID0gbnVsbDtcbiAgICB9XG5cbiAgICBpZiAoc2hvd0hhbm9pQm91bmRhcnkpIHtcbiAgICAgIGNvbnN0IG91dGVyUG9seSA9IEwucG9seWdvbihIQU5PSV9PVVRFUl9QT0xZR09OLCB7XG4gICAgICAgIGNvbG9yOiAnI2ZiYmYyNCcsXG4gICAgICAgIHdlaWdodDogMi41LFxuICAgICAgICBvcGFjaXR5OiAwLjksXG4gICAgICAgIGZpbGxDb2xvcjogJyNmYmJmMjQnLFxuICAgICAgICBmaWxsT3BhY2l0eTogMC4wNSxcbiAgICAgICAgZGFzaEFycmF5OiAnNSwgNScsXG4gICAgICB9KS5hZGRUbyhtYXBSZWYuY3VycmVudCk7XG4gICAgICBoYW5vaUJvdW5kYXJ5TGF5ZXJSZWYuY3VycmVudCA9IG91dGVyUG9seTtcbiAgICB9XG5cbiAgICAvLyBSZW5kZXIgRGlzdHJpY3QgcG9seWdvbnNcbiAgICBpZiAoZGlzdHJpY3RzTGF5ZXJSZWYuY3VycmVudCkge1xuICAgICAgZGlzdHJpY3RzTGF5ZXJSZWYuY3VycmVudC5jbGVhckxheWVycygpO1xuXG4gICAgICBpZiAoc2hvd0Rpc3RyaWN0cykge1xuICAgICAgICBIQU5PSV9ESVNUUklDVFMuZm9yRWFjaCgoZCkgPT4ge1xuICAgICAgICAgIGNvbnN0IHBvbHkgPSBMLnBvbHlnb24oZC5wb2x5Z29uLCB7XG4gICAgICAgICAgICBjb2xvcjogJyMzOGJkZjgnLFxuICAgICAgICAgICAgd2VpZ2h0OiAxLjIsXG4gICAgICAgICAgICBvcGFjaXR5OiAwLjYsXG4gICAgICAgICAgICBmaWxsQ29sb3I6ICcjMDI4NGM3JyxcbiAgICAgICAgICAgIGZpbGxPcGFjaXR5OiBzZWxlY3RlZERpc3RyaWN0Py5jb2RlID09PSBkLmNvZGUgPyAwLjM1IDogMC4wOCxcbiAgICAgICAgICB9KTtcblxuICAgICAgICAgIC8vIERpc3RyaWN0IGxhYmVsIC8gdG9vbHRpcFxuICAgICAgICAgIGlmIChzaG93TGFiZWxzKSB7XG4gICAgICAgICAgICBwb2x5LmJpbmRUb29sdGlwKFxuICAgICAgICAgICAgICBgPGRpdiBjbGFzcz1cInRleHQtWzExcHhdIGZvbnQtYm9sZCB0ZXh0LXNreS0zMDAgZHJvcC1zaGFkb3dcIj4ke2Quc2hvcnROYW1lfTwvZGl2PmAsXG4gICAgICAgICAgICAgIHtcbiAgICAgICAgICAgICAgICBwZXJtYW5lbnQ6IHpvb21MZXZlbCA+PSAxMSxcbiAgICAgICAgICAgICAgICBkaXJlY3Rpb246ICdjZW50ZXInLFxuICAgICAgICAgICAgICAgIGNsYXNzTmFtZTogJ2Rpc3RyaWN0LXRvb2x0aXAgYmctdHJhbnNwYXJlbnQgYm9yZGVyLTAgc2hhZG93LW5vbmUgdGV4dC1jZW50ZXInLFxuICAgICAgICAgICAgICB9XG4gICAgICAgICAgICApO1xuICAgICAgICAgIH1cblxuICAgICAgICAgIHBvbHkub24oJ2NsaWNrJywgKGUpID0+IHtcbiAgICAgICAgICAgIEwuRG9tRXZlbnQuc3RvcFByb3BhZ2F0aW9uKGUpO1xuICAgICAgICAgICAgc2V0U2VsZWN0ZWREaXN0cmljdChkKTtcbiAgICAgICAgICB9KTtcblxuICAgICAgICAgIHBvbHkuYWRkVG8oZGlzdHJpY3RzTGF5ZXJSZWYuY3VycmVudCEpO1xuICAgICAgICB9KTtcbiAgICAgIH1cbiAgICB9XG4gIH0sIFtzaG93RGlzdHJpY3RzLCBzaG93TGFiZWxzLCBzaG93SGFub2lCb3VuZGFyeSwgem9vbUxldmVsLCBzZWxlY3RlZERpc3RyaWN0XSk7XG5cbiAgLy8gNC4gUmVuZGVyIE1pbGl0YXJ5IFVuaXRzIE1hcmtlcnNcbiAgdXNlRWZmZWN0KCgpID0+IHtcbiAgICBpZiAoIW1hcFJlZi5jdXJyZW50IHx8ICF1bml0c0xheWVyUmVmLmN1cnJlbnQpIHJldHVybjtcbiAgICB1bml0c0xheWVyUmVmLmN1cnJlbnQuY2xlYXJMYXllcnMoKTtcblxuICAgIGlmICghc2hvd1VuaXRzKSByZXR1cm47XG5cbiAgICAvLyBGaWx0ZXIgdW5pdHMgaWYgYnJhbmNoIGZpbHRlciBhY3RpdmVcbiAgICBjb25zdCBmaWx0ZXJlZFVuaXRzID0gZmlsdGVyQnJhbmNoXG4gICAgICA/IHVuaXRzLmZpbHRlcigodSkgPT4gdS5icmFuY2ggPT09IGZpbHRlckJyYW5jaClcbiAgICAgIDogdW5pdHM7XG5cbiAgICBmaWx0ZXJlZFVuaXRzLmZvckVhY2goKHVuaXQpID0+IHtcbiAgICAgIGNvbnN0IGlzU2VsZWN0ZWQgPSBzZWxlY3RlZFVuaXQ/LmlkID09PSB1bml0LmlkO1xuICAgICAgY29uc3QgYmFkZ2UgPSBnZXRCcmFuY2hCYWRnZVN0eWxlKHVuaXQuYnJhbmNoKTtcblxuICAgICAgLy8gQ3VzdG9tIFNWRyBNYXJrZXIgd2l0aCBNaWxpdGFyeSBQQVZOIGRlc2lnblxuICAgICAgY29uc3QgY3VzdG9tSWNvbiA9IEwuZGl2SWNvbih7XG4gICAgICAgIGNsYXNzTmFtZTogJ2N1c3RvbS1taWxpdGFyeS1tYXJrZXInLFxuICAgICAgICBodG1sOiBgXG4gICAgICAgICAgPGRpdiBjbGFzcz1cInJlbGF0aXZlIGdyb3VwIGN1cnNvci1wb2ludGVyIGZsZXggZmxleC1jb2wgaXRlbXMtY2VudGVyXCI+XG4gICAgICAgICAgICAke1xuICAgICAgICAgICAgICBpc1NlbGVjdGVkXG4gICAgICAgICAgICAgICAgPyBgPGRpdiBjbGFzcz1cImFic29sdXRlIC1pbnNldC0yIHJvdW5kZWQtZnVsbCBib3JkZXItMiBib3JkZXItZW1lcmFsZC00MDAgcmFkYXItcGluZ1wiPjwvZGl2PmBcbiAgICAgICAgICAgICAgICA6ICcnXG4gICAgICAgICAgICB9XG4gICAgICAgICAgICA8ZGl2IHN0eWxlPVwiYmFja2dyb3VuZC1jb2xvcjogJHtiYWRnZS5iZ307IGJvcmRlcjogMnB4IHNvbGlkICR7XG4gICAgICAgICAgaXNTZWxlY3RlZCA/ICcjZmZmZmZmJyA6ICcjZmJiZjI0J1xuICAgICAgICB9O1wiIGNsYXNzPVwidy03IGgtNyByb3VuZGVkLWZ1bGwgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1jZW50ZXIgdGV4dC13aGl0ZSBmb250LWJvbGQgdGV4dC14cyBzaGFkb3ctbGcgdHJhbnNmb3JtIHRyYW5zaXRpb24tdHJhbnNmb3JtIGR1cmF0aW9uLTIwMCBncm91cC1ob3ZlcjpzY2FsZS0xMjVcIj5cbiAgICAgICAgICAgICAgPHNwYW4+JHtiYWRnZS5zeW1ib2x9PC9zcGFuPlxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICAke1xuICAgICAgICAgICAgICBzaG93TGFiZWxzXG4gICAgICAgICAgICAgICAgPyBgPGRpdiBjbGFzcz1cIm10LTEgcHgtMS41IHB5LTAuNSByb3VuZGVkIGJnLXNsYXRlLTkwMC85MCBib3JkZXIgYm9yZGVyLXNsYXRlLTcwMC84MCB0ZXh0LVsxMHB4XSBmb250LXNlbWlib2xkIHRleHQtc2xhdGUtMjAwIHdoaXRlc3BhY2Utbm93cmFwIHNoYWRvdy1tZCBwb2ludGVyLWV2ZW50cy1ub25lXCI+XG4gICAgICAgICAgICAgICAgICAgICR7dW5pdC5zaG9ydE5hbWV9XG4gICAgICAgICAgICAgICAgICAgPC9kaXY+YFxuICAgICAgICAgICAgICAgIDogJydcbiAgICAgICAgICAgIH1cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgYCxcbiAgICAgICAgaWNvblNpemU6IFsyOCwgMjhdLFxuICAgICAgICBpY29uQW5jaG9yOiBbMTQsIDE0XSxcbiAgICAgICAgcG9wdXBBbmNob3I6IFswLCAtMTZdLFxuICAgICAgfSk7XG5cbiAgICAgIGNvbnN0IG1hcmtlciA9IEwubWFya2VyKFt1bml0LmxhdGl0dWRlLCB1bml0LmxvbmdpdHVkZV0sIHsgaWNvbjogY3VzdG9tSWNvbiB9KTtcblxuICAgICAgLy8gQ2xpY2sgdG8gc2VsZWN0XG4gICAgICBtYXJrZXIub24oJ2NsaWNrJywgKGUpID0+IHtcbiAgICAgICAgTC5Eb21FdmVudC5zdG9wUHJvcGFnYXRpb24oZSk7XG4gICAgICAgIG9uU2VsZWN0VW5pdCh1bml0KTtcbiAgICAgIH0pO1xuXG4gICAgICAvLyBJbnRlcmFjdGl2ZSBQb3B1cFxuICAgICAgbWFya2VyLmJpbmRQb3B1cChgXG4gICAgICAgIDxkaXYgY2xhc3M9XCJwLTEgbWluLXctWzI0MHB4XVwiPlxuICAgICAgICAgIDxkaXYgY2xhc3M9XCJmbGV4IGl0ZW1zLWNlbnRlciBnYXAtMiBib3JkZXItYiBib3JkZXItc2xhdGUtNzAwIHBiLTIgbWItMlwiPlxuICAgICAgICAgICAgPHNwYW4gY2xhc3M9XCJ3LTMgaC0zIHJvdW5kZWQtZnVsbFwiIHN0eWxlPVwiYmFja2dyb3VuZC1jb2xvcjogJHtiYWRnZS5iZ31cIj48L3NwYW4+XG4gICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICA8ZGl2IGNsYXNzPVwidGV4dC1bMTFweF0gZm9udC1tb25vIHRleHQtZW1lcmFsZC00MDAgZm9udC1ib2xkXCI+JHt1bml0LmNvZGV9PC9kaXY+XG4gICAgICAgICAgICAgIDxoNCBjbGFzcz1cInRleHQtc20gZm9udC1ib2xkIHRleHQtd2hpdGUgbGVhZGluZy10aWdodFwiPiR7dW5pdC5uYW1lfTwvaDQ+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8ZGl2IGNsYXNzPVwic3BhY2UteS0xLjUgdGV4dC14cyB0ZXh0LXNsYXRlLTMwMFwiPlxuICAgICAgICAgICAgPGRpdj48c3BhbiBjbGFzcz1cInRleHQtc2xhdGUtNDAwXCI+QmluaCBjaOG7p25nOjwvc3Bhbj4gPHN0cm9uZyBjbGFzcz1cInRleHQtd2hpdGVcIj4ke3VuaXQuYnJhbmNofTwvc3Ryb25nPjwvZGl2PlxuICAgICAgICAgICAgPGRpdj48c3BhbiBjbGFzcz1cInRleHQtc2xhdGUtNDAwXCI+UXXhuq1uL0h1eeG7h246PC9zcGFuPiA8c3Ryb25nIGNsYXNzPVwidGV4dC13aGl0ZVwiPiR7dW5pdC5kaXN0cmljdH08L3N0cm9uZz48L2Rpdj5cbiAgICAgICAgICAgIDxkaXY+PHNwYW4gY2xhc3M9XCJ0ZXh0LXNsYXRlLTQwMFwiPlRy4buxYyBiYW46PC9zcGFuPiA8c3BhbiBjbGFzcz1cImZvbnQtbW9ubyB0ZXh0LWVtZXJhbGQtMzAwXCI+JHt1bml0LnBob25lfTwvc3Bhbj48L2Rpdj5cbiAgICAgICAgICAgIDxkaXY+PHNwYW4gY2xhc3M9XCJ0ZXh0LXNsYXRlLTQwMFwiPlThu41hIMSR4buZIFZOLTIwMDA6PC9zcGFuPiA8c3BhbiBjbGFzcz1cImZvbnQtbW9ubyB0ZXh0LXNreS0zMDAgdGV4dC1bMTFweF1cIj5YPSR7dW5pdC54X3ZuMjAwMC50b0xvY2FsZVN0cmluZygpfW0sIFk9JHt1bml0Lnlfdm4yMDAwLnRvTG9jYWxlU3RyaW5nKCl9bTwvc3Bhbj48L2Rpdj5cbiAgICAgICAgICAgIDxkaXYgY2xhc3M9XCJtdC0yIHB0LTIgYm9yZGVyLXQgYm9yZGVyLXNsYXRlLTgwMCBmbGV4IGp1c3RpZnktYmV0d2VlbiBpdGVtcy1jZW50ZXJcIj5cbiAgICAgICAgICAgICAgPHNwYW4gY2xhc3M9XCJ0ZXh0LVsxMHB4XSBweC0xLjUgcHktMC41IHJvdW5kZWQgJHtcbiAgICAgICAgICAgICAgICB1bml0LnJlYWRpbmVzc1N0YXR1cy5pbmNsdWRlcygnQ+G6pXAgMScpXG4gICAgICAgICAgICAgICAgICA/ICdiZy1yb3NlLTk1MCB0ZXh0LXJvc2UtMzAwIGJvcmRlciBib3JkZXItcm9zZS04MDAnXG4gICAgICAgICAgICAgICAgICA6ICdiZy1lbWVyYWxkLTk1MCB0ZXh0LWVtZXJhbGQtMzAwIGJvcmRlciBib3JkZXItZW1lcmFsZC04MDAnXG4gICAgICAgICAgICAgIH1cIj4ke3VuaXQucmVhZGluZXNzU3RhdHVzfTwvc3Bhbj5cbiAgICAgICAgICAgICAgPHNwYW4gY2xhc3M9XCJ0ZXh0LVsxMHB4XSB0ZXh0LWFtYmVyLTQwMFwiPsSQ4buZIG3huq10OiAke3VuaXQuc2VjdXJpdHlMZXZlbH08L3NwYW4+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgPC9kaXY+XG4gICAgICBgKTtcblxuICAgICAgbWFya2VyLmFkZFRvKHVuaXRzTGF5ZXJSZWYuY3VycmVudCEpO1xuICAgIH0pO1xuICB9LCBbdW5pdHMsIHNlbGVjdGVkVW5pdCwgc2hvd1VuaXRzLCBzaG93TGFiZWxzLCBmaWx0ZXJCcmFuY2hdKTtcblxuICAvLyA1LiBIYW5kbGUgU2VsZWN0ZWQgVW5pdCBwYW4vZmx5VG9cbiAgdXNlRWZmZWN0KCgpID0+IHtcbiAgICBpZiAoc2VsZWN0ZWRVbml0ICYmIG1hcFJlZi5jdXJyZW50KSB7XG4gICAgICBtYXBSZWYuY3VycmVudC5mbHlUbyhbc2VsZWN0ZWRVbml0LmxhdGl0dWRlLCBzZWxlY3RlZFVuaXQubG9uZ2l0dWRlXSwgMTQsIHtcbiAgICAgICAgZHVyYXRpb246IDEuMixcbiAgICAgIH0pO1xuICAgIH1cbiAgfSwgW3NlbGVjdGVkVW5pdF0pO1xuXG4gIC8vIDYuIFJlbmRlciBCdWZmZXIgWm9uZSBBbmFseXNpc1xuICB1c2VFZmZlY3QoKCkgPT4ge1xuICAgIGlmICghbWFwUmVmLmN1cnJlbnQgfHwgIWJ1ZmZlckxheWVyUmVmLmN1cnJlbnQpIHJldHVybjtcbiAgICBidWZmZXJMYXllclJlZi5jdXJyZW50LmNsZWFyTGF5ZXJzKCk7XG5cbiAgICBpZiAoYnVmZmVyQ29uZmlnLmFjdGl2ZSAmJiBidWZmZXJDb25maWcuY2VudGVyTGF0ICYmIGJ1ZmZlckNvbmZpZy5jZW50ZXJMbmcpIHtcbiAgICAgIGNvbnN0IGJ1ZmZlckNvb3JkcyA9IGNyZWF0ZUJ1ZmZlclBvbHlnb24oXG4gICAgICAgIGJ1ZmZlckNvbmZpZy5jZW50ZXJMYXQsXG4gICAgICAgIGJ1ZmZlckNvbmZpZy5jZW50ZXJMbmcsXG4gICAgICAgIGJ1ZmZlckNvbmZpZy5yYWRpdXNLbVxuICAgICAgKTtcblxuICAgICAgLy8gQ2lyY2xlIEJ1ZmZlciBQb2x5Z29uXG4gICAgICBjb25zdCBidWZmZXJQb2x5ID0gTC5wb2x5Z29uKGJ1ZmZlckNvb3Jkcywge1xuICAgICAgICBjb2xvcjogJyNmNTllMGInLFxuICAgICAgICB3ZWlnaHQ6IDIsXG4gICAgICAgIGRhc2hBcnJheTogJzYsIDYnLFxuICAgICAgICBmaWxsQ29sb3I6ICcjZjU5ZTBiJyxcbiAgICAgICAgZmlsbE9wYWNpdHk6IDAuMTUsXG4gICAgICB9KTtcblxuICAgICAgLy8gQ2VudGVyIGNyb3NzaGFpciBtYXJrZXJcbiAgICAgIGNvbnN0IGNlbnRlck1hcmtlciA9IEwuY2lyY2xlTWFya2VyKFtidWZmZXJDb25maWcuY2VudGVyTGF0LCBidWZmZXJDb25maWcuY2VudGVyTG5nXSwge1xuICAgICAgICByYWRpdXM6IDYsXG4gICAgICAgIGNvbG9yOiAnI2VmNDQ0NCcsXG4gICAgICAgIGZpbGxDb2xvcjogJyNmZmZmZmYnLFxuICAgICAgICBmaWxsT3BhY2l0eTogMSxcbiAgICAgICAgd2VpZ2h0OiAyLFxuICAgICAgfSk7XG5cbiAgICAgIGJ1ZmZlclBvbHkuYmluZFRvb2x0aXAoXG4gICAgICAgIGBWw7luZyDEkeG7h20gYuG6o28gduG7hzogUiA9ICR7YnVmZmVyQ29uZmlnLnJhZGl1c0ttfSBrbSAoJHtidWZmZXJDb25maWcudGFyZ2V0VW5pdE5hbWUgfHwgJyd9KWAsXG4gICAgICAgIHsgcGVybWFuZW50OiB0cnVlLCBkaXJlY3Rpb246ICd0b3AnIH1cbiAgICAgICk7XG5cbiAgICAgIGJ1ZmZlclBvbHkuYWRkVG8oYnVmZmVyTGF5ZXJSZWYuY3VycmVudCk7XG4gICAgICBjZW50ZXJNYXJrZXIuYWRkVG8oYnVmZmVyTGF5ZXJSZWYuY3VycmVudCk7XG4gICAgfVxuICB9LCBbYnVmZmVyQ29uZmlnXSk7XG5cbiAgLy8gNy4gUmVuZGVyIE1lYXN1cmVtZW50IFBvbHlsaW5lXG4gIHVzZUVmZmVjdCgoKSA9PiB7XG4gICAgaWYgKCFtYXBSZWYuY3VycmVudCB8fCAhbWVhc3VyZUxheWVyUmVmLmN1cnJlbnQpIHJldHVybjtcbiAgICBtZWFzdXJlTGF5ZXJSZWYuY3VycmVudC5jbGVhckxheWVycygpO1xuXG4gICAgaWYgKG1lYXN1cmVQb2ludHMubGVuZ3RoID4gMCkge1xuICAgICAgLy8gRHJhdyBwb2ludHNcbiAgICAgIG1lYXN1cmVQb2ludHMuZm9yRWFjaCgocHQsIGlkeCkgPT4ge1xuICAgICAgICBMLmNpcmNsZU1hcmtlcihwdCwge1xuICAgICAgICAgIHJhZGl1czogNSxcbiAgICAgICAgICBjb2xvcjogJyMzOGJkZjgnLFxuICAgICAgICAgIGZpbGxDb2xvcjogaWR4ID09PSAwID8gJyMxMGI5ODEnIDogJyNmNTllMGInLFxuICAgICAgICAgIGZpbGxPcGFjaXR5OiAxLFxuICAgICAgICAgIHdlaWdodDogMixcbiAgICAgICAgfSkuYWRkVG8obWVhc3VyZUxheWVyUmVmLmN1cnJlbnQhKTtcbiAgICAgIH0pO1xuXG4gICAgICAvLyBEcmF3IHBvbHlsaW5lXG4gICAgICBpZiAobWVhc3VyZVBvaW50cy5sZW5ndGggPj0gMikge1xuICAgICAgICBMLnBvbHlsaW5lKG1lYXN1cmVQb2ludHMsIHtcbiAgICAgICAgICBjb2xvcjogJyMzOGJkZjgnLFxuICAgICAgICAgIHdlaWdodDogMyxcbiAgICAgICAgICBkYXNoQXJyYXk6ICc0LCA0JyxcbiAgICAgICAgfSkuYWRkVG8obWVhc3VyZUxheWVyUmVmLmN1cnJlbnQhKTtcbiAgICAgIH1cbiAgICB9XG4gIH0sIFttZWFzdXJlUG9pbnRzXSk7XG5cbiAgLy8gVG9vbCBhY3Rpb25zXG4gIGNvbnN0IGhhbmRsZVpvb21JbiA9ICgpID0+IG1hcFJlZi5jdXJyZW50Py56b29tSW4oKTtcbiAgY29uc3QgaGFuZGxlWm9vbU91dCA9ICgpID0+IG1hcFJlZi5jdXJyZW50Py56b29tT3V0KCk7XG4gIGNvbnN0IGhhbmRsZUZpdEV4dGVudCA9ICgpID0+IHtcbiAgICBtYXBSZWYuY3VycmVudD8uZml0Qm91bmRzKEhBTk9JX0JPVU5EUyk7XG4gIH07XG4gIGNvbnN0IGhhbmRsZUNsZWFyTWVhc3VyZW1lbnQgPSAoKSA9PiB7XG4gICAgc2V0TWVhc3VyZVBvaW50cyhbXSk7XG4gICAgc2V0TWVhc3VyZWREaXN0YW5jZShudWxsKTtcbiAgICBvblNldEJ1ZmZlckNvbmZpZyh7IGFjdGl2ZTogZmFsc2UsIGNlbnRlckxhdDogMCwgY2VudGVyTG5nOiAwLCByYWRpdXNLbTogMTAgfSk7XG4gICAgc2V0U2VsZWN0ZWREaXN0cmljdChudWxsKTtcbiAgICBvblNlbGVjdFVuaXQobnVsbCk7XG4gIH07XG5cbiAgcmV0dXJuIChcbiAgICA8ZGl2IGNsYXNzTmFtZT1cInJlbGF0aXZlIHctZnVsbCBoLWZ1bGwgZmxleC0xIG92ZXJmbG93LWhpZGRlbiBiZy1bIzBiMTExZV1cIj5cbiAgICAgIHsvKiBNYXAgQ29udGFpbmVyICovfVxuICAgICAgPGRpdiByZWY9e21hcENvbnRhaW5lclJlZn0gY2xhc3NOYW1lPVwidy1mdWxsIGgtZnVsbFwiIC8+XG5cbiAgICAgIHsvKiBUb3AtTGVmdCBHSVMgVG9vbGJhciAobWF0Y2hpbmcgaW1hZ2UucG5nKSAqL31cbiAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYWJzb2x1dGUgdG9wLTQgbGVmdC00IHotWzEwMDBdIGZsZXggZmxleC1jb2wgZ2FwLTEuNSBiZy1bIzBiMTMyNF0vOTAgYmFja2Ryb3AtYmx1ci1tZCBwLTEuNSByb3VuZGVkLWxnIGJvcmRlciBib3JkZXItc2xhdGUtNzAwLzgwIHNoYWRvdy0yeGxcIj5cbiAgICAgICAgPGJ1dHRvblxuICAgICAgICAgIG9uQ2xpY2s9e2hhbmRsZVpvb21Jbn1cbiAgICAgICAgICB0aXRsZT1cIlBow7NuZyB0byBi4bqjbiDEkeG7kyAoKylcIlxuICAgICAgICAgIGNsYXNzTmFtZT1cInctOCBoLTggcm91bmRlZCBiZy1zbGF0ZS04MDAgaG92ZXI6YmctZW1lcmFsZC02MDAgdGV4dC1zbGF0ZS0yMDAgaG92ZXI6dGV4dC13aGl0ZSBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWNlbnRlciB0cmFuc2l0aW9uXCJcbiAgICAgICAgPlxuICAgICAgICAgIDxab29tSW4gY2xhc3NOYW1lPVwidy00IGgtNFwiIC8+XG4gICAgICAgIDwvYnV0dG9uPlxuICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgb25DbGljaz17aGFuZGxlWm9vbU91dH1cbiAgICAgICAgICB0aXRsZT1cIlRodSBuaOG7jyBi4bqjbiDEkeG7kyAoLSlcIlxuICAgICAgICAgIGNsYXNzTmFtZT1cInctOCBoLTggcm91bmRlZCBiZy1zbGF0ZS04MDAgaG92ZXI6YmctZW1lcmFsZC02MDAgdGV4dC1zbGF0ZS0yMDAgaG92ZXI6dGV4dC13aGl0ZSBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWNlbnRlciB0cmFuc2l0aW9uXCJcbiAgICAgICAgPlxuICAgICAgICAgIDxab29tT3V0IGNsYXNzTmFtZT1cInctNCBoLTRcIiAvPlxuICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgPGJ1dHRvblxuICAgICAgICAgIG9uQ2xpY2s9e2hhbmRsZUZpdEV4dGVudH1cbiAgICAgICAgICB0aXRsZT1cIlhlbSB0b8OgbiBi4buZIGzDo25oIHRo4buVIEjDoCBO4buZaSAoRnVsbCBFeHRlbnQpXCJcbiAgICAgICAgICBjbGFzc05hbWU9XCJ3LTggaC04IHJvdW5kZWQgYmctc2xhdGUtODAwIGhvdmVyOmJnLWVtZXJhbGQtNjAwIHRleHQtc2xhdGUtMjAwIGhvdmVyOnRleHQtd2hpdGUgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1jZW50ZXIgdHJhbnNpdGlvblwiXG4gICAgICAgID5cbiAgICAgICAgICA8TWF4aW1pemUyIGNsYXNzTmFtZT1cInctNCBoLTRcIiAvPlxuICAgICAgICA8L2J1dHRvbj5cblxuICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cInctZnVsbCBoLXB4IGJnLXNsYXRlLTcwMCBteS0wLjVcIiAvPlxuXG4gICAgICAgIDxidXR0b25cbiAgICAgICAgICBvbkNsaWNrPXsoKSA9PiBzZXRBY3RpdmVUb29sKGFjdGl2ZVRvb2wgPT09ICdpZGVudGlmeScgPyAncGFuJyA6ICdpZGVudGlmeScpfVxuICAgICAgICAgIHRpdGxlPVwiVHJhIGPhu6l1IHRow7RuZyB0aW4gxJHhu5FpIHTGsOG7o25nIChJZGVudGlmeSlcIlxuICAgICAgICAgIGNsYXNzTmFtZT17YHctOCBoLTggcm91bmRlZCBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWNlbnRlciB0cmFuc2l0aW9uICR7XG4gICAgICAgICAgICBhY3RpdmVUb29sID09PSAnaWRlbnRpZnknXG4gICAgICAgICAgICAgID8gJ2JnLXB1cnBsZS02MDAgdGV4dC13aGl0ZSByaW5nLTIgcmluZy1wdXJwbGUtNDAwJ1xuICAgICAgICAgICAgICA6ICdiZy1zbGF0ZS04MDAgaG92ZXI6YmctcHVycGxlLTcwMCB0ZXh0LXNsYXRlLTIwMCdcbiAgICAgICAgICB9YH1cbiAgICAgICAgPlxuICAgICAgICAgIDxJbmZvIGNsYXNzTmFtZT1cInctNCBoLTRcIiAvPlxuICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgPGJ1dHRvblxuICAgICAgICAgIG9uQ2xpY2s9eygpID0+IHNldEFjdGl2ZVRvb2woYWN0aXZlVG9vbCA9PT0gJ21lYXN1cmVfZGlzdGFuY2UnID8gJ3BhbicgOiAnbWVhc3VyZV9kaXN0YW5jZScpfVxuICAgICAgICAgIHRpdGxlPVwixJBvIGtob+G6o25nIGPDoWNoIHTDoWMgY2hp4bq/biBj4buxIGx5IChNZWFzdXJlIFRvb2wpXCJcbiAgICAgICAgICBjbGFzc05hbWU9e2B3LTggaC04IHJvdW5kZWQgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1jZW50ZXIgdHJhbnNpdGlvbiAke1xuICAgICAgICAgICAgYWN0aXZlVG9vbCA9PT0gJ21lYXN1cmVfZGlzdGFuY2UnXG4gICAgICAgICAgICAgID8gJ2JnLXNreS02MDAgdGV4dC13aGl0ZSByaW5nLTIgcmluZy1za3ktNDAwJ1xuICAgICAgICAgICAgICA6ICdiZy1zbGF0ZS04MDAgaG92ZXI6Ymctc2t5LTcwMCB0ZXh0LXNsYXRlLTIwMCdcbiAgICAgICAgICB9YH1cbiAgICAgICAgPlxuICAgICAgICAgIDxSdWxlciBjbGFzc05hbWU9XCJ3LTQgaC00XCIgLz5cbiAgICAgICAgPC9idXR0b24+XG4gICAgICAgIDxidXR0b25cbiAgICAgICAgICBvbkNsaWNrPXsoKSA9PiBzZXRBY3RpdmVUb29sKGFjdGl2ZVRvb2wgPT09ICdidWZmZXInID8gJ3BhbicgOiAnYnVmZmVyJyl9XG4gICAgICAgICAgdGl0bGU9XCJU4bqhbyB2w7luZyDEkeG7h20gYuG6o28gduG7hyBiw6FuIGvDrW5oIChCdWZmZXIgWm9uZSlcIlxuICAgICAgICAgIGNsYXNzTmFtZT17YHctOCBoLTggcm91bmRlZCBmbGV4IGl0ZW1zLWNlbnRlciBqdXN0aWZ5LWNlbnRlciB0cmFuc2l0aW9uICR7XG4gICAgICAgICAgICBhY3RpdmVUb29sID09PSAnYnVmZmVyJ1xuICAgICAgICAgICAgICA/ICdiZy1hbWJlci02MDAgdGV4dC13aGl0ZSByaW5nLTIgcmluZy1hbWJlci00MDAnXG4gICAgICAgICAgICAgIDogJ2JnLXNsYXRlLTgwMCBob3ZlcjpiZy1hbWJlci03MDAgdGV4dC1zbGF0ZS0yMDAnXG4gICAgICAgICAgfWB9XG4gICAgICAgID5cbiAgICAgICAgICA8Q2lyY2xlRG90IGNsYXNzTmFtZT1cInctNCBoLTRcIiAvPlxuICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgPGJ1dHRvblxuICAgICAgICAgIG9uQ2xpY2s9eygpID0+IHNldFNob3dMYXllclBhbmVsKCFzaG93TGF5ZXJQYW5lbCl9XG4gICAgICAgICAgdGl0bGU9XCJRdeG6o24gbMO9IGzhu5twIGThu68gbGnhu4d1IEdJUyAoTGF5ZXIgU3RhY2spXCJcbiAgICAgICAgICBjbGFzc05hbWU9e2B3LTggaC04IHJvdW5kZWQgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1jZW50ZXIgdHJhbnNpdGlvbiAke1xuICAgICAgICAgICAgc2hvd0xheWVyUGFuZWxcbiAgICAgICAgICAgICAgPyAnYmctZW1lcmFsZC02MDAgdGV4dC13aGl0ZSdcbiAgICAgICAgICAgICAgOiAnYmctc2xhdGUtODAwIGhvdmVyOmJnLWVtZXJhbGQtNzAwIHRleHQtc2xhdGUtMjAwJ1xuICAgICAgICAgIH1gfVxuICAgICAgICA+XG4gICAgICAgICAgPExheWVycyBjbGFzc05hbWU9XCJ3LTQgaC00XCIgLz5cbiAgICAgICAgPC9idXR0b24+XG4gICAgICAgIDxidXR0b25cbiAgICAgICAgICBvbkNsaWNrPXtoYW5kbGVDbGVhck1lYXN1cmVtZW50fVxuICAgICAgICAgIHRpdGxlPVwiTMOgbSBz4bqhY2ggcGjDqXAgxJFvIHbDoCB2w7luZyBjaOG7jW4gKENsZWFyIC8gUmVzZXQpXCJcbiAgICAgICAgICBjbGFzc05hbWU9XCJ3LTggaC04IHJvdW5kZWQgYmctc2xhdGUtODAwIGhvdmVyOmJnLXJvc2UtNzAwIHRleHQtc2xhdGUtMjAwIGhvdmVyOnRleHQtd2hpdGUgZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1jZW50ZXIgdHJhbnNpdGlvblwiXG4gICAgICAgID5cbiAgICAgICAgICA8Um90YXRlQ2N3IGNsYXNzTmFtZT1cInctNCBoLTRcIiAvPlxuICAgICAgICA8L2J1dHRvbj5cbiAgICAgIDwvZGl2PlxuXG4gICAgICB7LyogVmVydGljYWwgWm9vbSBTbGlkZXIgKG1hdGNoaW5nIGltYWdlLnBuZyBiZWxvdyB0b29sYmFyKSAqL31cbiAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYWJzb2x1dGUgdG9wLTcyIGxlZnQtNSB6LVsxMDAwXSBoaWRkZW4gc206ZmxleCBmbGV4LWNvbCBpdGVtcy1jZW50ZXIgYmctWyMwYjEzMjRdLzkwIGJhY2tkcm9wLWJsdXItbWQgcHgtMS41IHB5LTMgcm91bmRlZC1mdWxsIGJvcmRlciBib3JkZXItc2xhdGUtNzAwIHNoYWRvdy14bCBnYXAtMlwiPlxuICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgb25DbGljaz17aGFuZGxlWm9vbUlufVxuICAgICAgICAgIGNsYXNzTmFtZT1cInRleHQteHMgZm9udC1ib2xkIHRleHQtc2xhdGUtMzAwIGhvdmVyOnRleHQtZW1lcmFsZC00MDBcIlxuICAgICAgICA+XG4gICAgICAgICAgK1xuICAgICAgICA8L2J1dHRvbj5cbiAgICAgICAgPGlucHV0XG4gICAgICAgICAgdHlwZT1cInJhbmdlXCJcbiAgICAgICAgICBtaW49ezl9XG4gICAgICAgICAgbWF4PXsxN31cbiAgICAgICAgICB2YWx1ZT17em9vbUxldmVsfVxuICAgICAgICAgIG9uQ2hhbmdlPXsoZSkgPT4gbWFwUmVmLmN1cnJlbnQ/LnNldFpvb20oTnVtYmVyKGUudGFyZ2V0LnZhbHVlKSl9XG4gICAgICAgICAgY2xhc3NOYW1lPVwidy0yNCBoLTEgYmctc2xhdGUtNzAwIHJvdW5kZWQtbGcgYXBwZWFyYW5jZS1ub25lIGN1cnNvci1wb2ludGVyIC1yb3RhdGUtOTAgbXktMTAgYWNjZW50LWVtZXJhbGQtNTAwXCJcbiAgICAgICAgLz5cbiAgICAgICAgPGJ1dHRvblxuICAgICAgICAgIG9uQ2xpY2s9e2hhbmRsZVpvb21PdXR9XG4gICAgICAgICAgY2xhc3NOYW1lPVwidGV4dC14cyBmb250LWJvbGQgdGV4dC1zbGF0ZS0zMDAgaG92ZXI6dGV4dC1lbWVyYWxkLTQwMFwiXG4gICAgICAgID5cbiAgICAgICAgICAtXG4gICAgICAgIDwvYnV0dG9uPlxuICAgICAgPC9kaXY+XG5cbiAgICAgIHsvKiBGbG9hdGluZyBMYXllciBDb250cm9sIFBhbmVsICovfVxuICAgICAge3Nob3dMYXllclBhbmVsICYmIChcbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJhYnNvbHV0ZSB0b3AtNCBsZWZ0LTE2IHotWzEwMDFdIHctNjQgYmctWyMwOTEwMWVdLzk1IGJhY2tkcm9wLWJsdXItbWQgYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAgcm91bmRlZC1sZyBwLTMgdGV4dC13aGl0ZSBzaGFkb3ctMnhsIHNwYWNlLXktM1wiPlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuIGJvcmRlci1iIGJvcmRlci1zbGF0ZS03MDAgcGItMlwiPlxuICAgICAgICAgICAgPGg0IGNsYXNzTmFtZT1cInRleHQteHMgZm9udC1ib2xkIHRleHQtZW1lcmFsZC00MDAgZmxleCBpdGVtcy1jZW50ZXIgZ2FwLTEuNVwiPlxuICAgICAgICAgICAgICA8TGF5ZXJzIGNsYXNzTmFtZT1cInctMy41IGgtMy41XCIgLz5cbiAgICAgICAgICAgICAgPHNwYW4+UVXhuqJOIEzDnSBM4buaUCBC4bqiTiDEkOG7kjwvc3Bhbj5cbiAgICAgICAgICAgIDwvaDQ+XG4gICAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICAgIG9uQ2xpY2s9eygpID0+IHNldFNob3dMYXllclBhbmVsKGZhbHNlKX1cbiAgICAgICAgICAgICAgY2xhc3NOYW1lPVwidGV4dC1zbGF0ZS00MDAgaG92ZXI6dGV4dC13aGl0ZVwiXG4gICAgICAgICAgICA+XG4gICAgICAgICAgICAgIDxYIGNsYXNzTmFtZT1cInctMy41IGgtMy41XCIgLz5cbiAgICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgIDwvZGl2PlxuXG4gICAgICAgICAgey8qIEJhc2VtYXAgc2VsZWN0b3IgKi99XG4gICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJzcGFjZS15LTFcIj5cbiAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtWzExcHhdIGZvbnQtc2VtaWJvbGQgdGV4dC1zbGF0ZS0zMDBcIj5C4bqjbiDEkeG7kyBu4buBbjo8L3NwYW4+XG4gICAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImdyaWQgZ3JpZC1jb2xzLTIgZ2FwLTEuNSBwdC0xXCI+XG4gICAgICAgICAgICAgIHsoT2JqZWN0LmtleXMoYmFzZW1hcFVybHMpIGFzIEJhc2VtYXBUeXBlW10pLm1hcCgoa2V5KSA9PiAoXG4gICAgICAgICAgICAgICAgPGJ1dHRvblxuICAgICAgICAgICAgICAgICAga2V5PXtrZXl9XG4gICAgICAgICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiBzZXRCYXNlbWFwKGtleSl9XG4gICAgICAgICAgICAgICAgICBjbGFzc05hbWU9e2B0ZXh0LVsxMHB4XSBwLTEuNSByb3VuZGVkIHRleHQtbGVmdCBib3JkZXIgJHtcbiAgICAgICAgICAgICAgICAgICAgYmFzZW1hcCA9PT0ga2V5XG4gICAgICAgICAgICAgICAgICAgICAgPyAnYmctZW1lcmFsZC05MDAvNjAgYm9yZGVyLWVtZXJhbGQtNTAwIHRleHQtZW1lcmFsZC0yMDAgZm9udC1ib2xkJ1xuICAgICAgICAgICAgICAgICAgICAgIDogJ2JnLXNsYXRlLTgwMCBib3JkZXItc2xhdGUtNzAwIHRleHQtc2xhdGUtMzAwIGhvdmVyOmJnLXNsYXRlLTcwMCdcbiAgICAgICAgICAgICAgICAgIH1gfVxuICAgICAgICAgICAgICAgID5cbiAgICAgICAgICAgICAgICAgIHtiYXNlbWFwVXJsc1trZXldLm5hbWV9XG4gICAgICAgICAgICAgICAgPC9idXR0b24+XG4gICAgICAgICAgICAgICkpfVxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgPC9kaXY+XG5cbiAgICAgICAgICB7LyogT3ZlcmxheSB0b2dnbGVzICovfVxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwic3BhY2UteS0yIGJvcmRlci10IGJvcmRlci1zbGF0ZS04MDAgcHQtMiB0ZXh0LXhzXCI+XG4gICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LVsxMXB4XSBmb250LXNlbWlib2xkIHRleHQtc2xhdGUtMzAwXCI+TOG7m3AgY2h1ecOqbiDEkeG7gTo8L3NwYW4+XG4gICAgICAgICAgICA8bGFiZWwgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuIGN1cnNvci1wb2ludGVyXCI+XG4gICAgICAgICAgICAgIDxzcGFuPsSQxqFuIHbhu4sgcXXDom4gxJHhu5lpPC9zcGFuPlxuICAgICAgICAgICAgICA8aW5wdXRcbiAgICAgICAgICAgICAgICB0eXBlPVwiY2hlY2tib3hcIlxuICAgICAgICAgICAgICAgIGNoZWNrZWQ9e3Nob3dVbml0c31cbiAgICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IHNldFNob3dVbml0cyhlLnRhcmdldC5jaGVja2VkKX1cbiAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJhY2NlbnQtZW1lcmFsZC01MDAgcm91bmRlZFwiXG4gICAgICAgICAgICAgIC8+XG4gICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgPGxhYmVsIGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlbiBjdXJzb3ItcG9pbnRlclwiPlxuICAgICAgICAgICAgICA8c3Bhbj5SYW5oIGdp4bubaSAzMCBRdeG6rW4vSHV54buHbjwvc3Bhbj5cbiAgICAgICAgICAgICAgPGlucHV0XG4gICAgICAgICAgICAgICAgdHlwZT1cImNoZWNrYm94XCJcbiAgICAgICAgICAgICAgICBjaGVja2VkPXtzaG93RGlzdHJpY3RzfVxuICAgICAgICAgICAgICAgIG9uQ2hhbmdlPXsoZSkgPT4gc2V0U2hvd0Rpc3RyaWN0cyhlLnRhcmdldC5jaGVja2VkKX1cbiAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJhY2NlbnQtZW1lcmFsZC01MDAgcm91bmRlZFwiXG4gICAgICAgICAgICAgIC8+XG4gICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgICAgPGxhYmVsIGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlbiBjdXJzb3ItcG9pbnRlclwiPlxuICAgICAgICAgICAgICA8c3Bhbj7EkMaw4budbmcgYmnDqm4gVFAuIEjDoCBO4buZaTwvc3Bhbj5cbiAgICAgICAgICAgICAgPGlucHV0XG4gICAgICAgICAgICAgICAgdHlwZT1cImNoZWNrYm94XCJcbiAgICAgICAgICAgICAgICBjaGVja2VkPXtzaG93SGFub2lCb3VuZGFyeX1cbiAgICAgICAgICAgICAgICBvbkNoYW5nZT17KGUpID0+IHNldFNob3dIYW5vaUJvdW5kYXJ5KGUudGFyZ2V0LmNoZWNrZWQpfVxuICAgICAgICAgICAgICAgIGNsYXNzTmFtZT1cImFjY2VudC1lbWVyYWxkLTUwMCByb3VuZGVkXCJcbiAgICAgICAgICAgICAgLz5cbiAgICAgICAgICAgIDwvbGFiZWw+XG4gICAgICAgICAgICA8bGFiZWwgY2xhc3NOYW1lPVwiZmxleCBpdGVtcy1jZW50ZXIganVzdGlmeS1iZXR3ZWVuIGN1cnNvci1wb2ludGVyXCI+XG4gICAgICAgICAgICAgIDxzcGFuPk5ow6NuIHTDqm4gxJHGoW4gduG7izwvc3Bhbj5cbiAgICAgICAgICAgICAgPGlucHV0XG4gICAgICAgICAgICAgICAgdHlwZT1cImNoZWNrYm94XCJcbiAgICAgICAgICAgICAgICBjaGVja2VkPXtzaG93TGFiZWxzfVxuICAgICAgICAgICAgICAgIG9uQ2hhbmdlPXsoZSkgPT4gc2V0U2hvd0xhYmVscyhlLnRhcmdldC5jaGVja2VkKX1cbiAgICAgICAgICAgICAgICBjbGFzc05hbWU9XCJhY2NlbnQtZW1lcmFsZC01MDAgcm91bmRlZFwiXG4gICAgICAgICAgICAgIC8+XG4gICAgICAgICAgICA8L2xhYmVsPlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICA8L2Rpdj5cbiAgICAgICl9XG5cbiAgICAgIHsvKiBNZWFzdXJlbWVudCBCYW5uZXIgKi99XG4gICAgICB7YWN0aXZlVG9vbCA9PT0gJ21lYXN1cmVfZGlzdGFuY2UnICYmIChcbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJhYnNvbHV0ZSB0b3AtNCBsZWZ0LTEvMiAtdHJhbnNsYXRlLXgtMS8yIHotWzEwMDBdIGJnLXNsYXRlLTkwMC85NSBib3JkZXIgYm9yZGVyLXNreS01MDAgcHgtNCBweS0yIHJvdW5kZWQtbGcgdGV4dC14cyB0ZXh0LXdoaXRlIHNoYWRvdy14bCBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtM1wiPlxuICAgICAgICAgIDxSdWxlciBjbGFzc05hbWU9XCJ3LTQgaC00IHRleHQtc2t5LTQwMFwiIC8+XG4gICAgICAgICAgPHNwYW4+QuG6pW0gY2h14buZdCB0csOqbiBi4bqjbiDEkeG7kyDEkeG7gyBjaOG7jW4gY8OhYyDEkWnhu4NtIMSRbyBj4buxIGx5IHTDoWMgY2hp4bq/bi48L3NwYW4+XG4gICAgICAgICAge21lYXN1cmVkRGlzdGFuY2UgIT09IG51bGwgJiYgKFxuICAgICAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJiZy1za3ktOTUwIHB4LTIuNSBweS0xIHJvdW5kZWQgYm9yZGVyIGJvcmRlci1za3ktNDAwIHRleHQtc2t5LTIwMCBmb250LW1vbm8gZm9udC1ib2xkXCI+XG4gICAgICAgICAgICAgIFThu5VuZyBj4buxIGx5OiB7bWVhc3VyZWREaXN0YW5jZX0ga20gKHtNYXRoLnJvdW5kKG1lYXN1cmVkRGlzdGFuY2UgKiAxMDAwKS50b0xvY2FsZVN0cmluZygpfSBtKVxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgKX1cbiAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiB7XG4gICAgICAgICAgICAgIHNldE1lYXN1cmVQb2ludHMoW10pO1xuICAgICAgICAgICAgICBzZXRNZWFzdXJlZERpc3RhbmNlKG51bGwpO1xuICAgICAgICAgICAgfX1cbiAgICAgICAgICAgIGNsYXNzTmFtZT1cInRleHQtWzExcHhdIHRleHQtcm9zZS00MDAgaG92ZXI6dGV4dC1yb3NlLTMwMCB1bmRlcmxpbmUgZm9udC1tZWRpdW0gbWwtMlwiXG4gICAgICAgICAgPlxuICAgICAgICAgICAgxJBvIGzhuqFpXG4gICAgICAgICAgPC9idXR0b24+XG4gICAgICAgIDwvZGl2PlxuICAgICAgKX1cblxuICAgICAgey8qIEJ1ZmZlciBab25lIFNldHVwIEJhbm5lciAqL31cbiAgICAgIHthY3RpdmVUb29sID09PSAnYnVmZmVyJyAmJiAoXG4gICAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYWJzb2x1dGUgdG9wLTQgbGVmdC0xLzIgLXRyYW5zbGF0ZS14LTEvMiB6LVsxMDAwXSBiZy1zbGF0ZS05MDAvOTUgYm9yZGVyIGJvcmRlci1hbWJlci01MDAgcHgtNCBweS0yIHJvdW5kZWQtbGcgdGV4dC14cyB0ZXh0LXdoaXRlIHNoYWRvdy14bCBmbGV4IGl0ZW1zLWNlbnRlciBnYXAtM1wiPlxuICAgICAgICAgIDxDaXJjbGVEb3QgY2xhc3NOYW1lPVwidy00IGgtNCB0ZXh0LWFtYmVyLTQwMFwiIC8+XG4gICAgICAgICAgPHNwYW4+QuG6pW0gxJFp4buDbSB0csOqbiBi4bqjbiDEkeG7kyDEkeG7gyB04bqhbyB2w7luZyDEkeG7h20gcGjDsm5nIHRo4bunLiBCw6FuIGvDrW5oOjwvc3Bhbj5cbiAgICAgICAgICA8c2VsZWN0XG4gICAgICAgICAgICB2YWx1ZT17YnVmZmVyQ29uZmlnLnJhZGl1c0ttfVxuICAgICAgICAgICAgb25DaGFuZ2U9eyhlKSA9PlxuICAgICAgICAgICAgICBvblNldEJ1ZmZlckNvbmZpZyh7IC4uLmJ1ZmZlckNvbmZpZywgcmFkaXVzS206IE51bWJlcihlLnRhcmdldC52YWx1ZSkgfSlcbiAgICAgICAgICAgIH1cbiAgICAgICAgICAgIGNsYXNzTmFtZT1cImJnLXNsYXRlLTgwMCBib3JkZXIgYm9yZGVyLXNsYXRlLTYwMCByb3VuZGVkIHB4LTIgcHktMSB0ZXh0LXhzIHRleHQtd2hpdGUgZm9udC1tb25vXCJcbiAgICAgICAgICA+XG4gICAgICAgICAgICA8b3B0aW9uIHZhbHVlPXszfT4zIGttIChQaOG6oW0gdmkgc8O6bmcgY+G7kWkvdOG6p20gZ+G6p24pPC9vcHRpb24+XG4gICAgICAgICAgICA8b3B0aW9uIHZhbHVlPXs1fT41IGttIChWw6BuaCDEkWFpIGFuIG5pbmggdHLhu7FjIHRp4bq/cCk8L29wdGlvbj5cbiAgICAgICAgICAgIDxvcHRpb24gdmFsdWU9ezEwfT4xMCBrbSAoVsOgbmggxJFhaSBwaMOybmcgdGjhu6cgY2hp4bq/biBk4buLY2gpPC9vcHRpb24+XG4gICAgICAgICAgICA8b3B0aW9uIHZhbHVlPXsyMH0+MjAga20gKFbDuW5nIGjhu49hIGzhu7FjIHBow6FvIGJpbmgpPC9vcHRpb24+XG4gICAgICAgICAgICA8b3B0aW9uIHZhbHVlPXszNX0+MzUga20gKFThuqdtIHTDoWMgY2hp4bq/biB0w6puIGzhu61hIHBow7JuZyBraMO0bmcgdOG6p20gdHJ1bmcpPC9vcHRpb24+XG4gICAgICAgICAgPC9zZWxlY3Q+XG4gICAgICAgIDwvZGl2PlxuICAgICAgKX1cblxuICAgICAgey8qIFNjYWxlIEJhciAobWF0Y2hpbmcgaW1hZ2UucG5nIGJvdHRvbS1sZWZ0KSAqL31cbiAgICAgIDxkaXYgY2xhc3NOYW1lPVwiYWJzb2x1dGUgYm90dG9tLTYgbGVmdC01IHotWzEwMDBdIGJnLVsjMDkxMDFmXS84NSBiYWNrZHJvcC1ibHVyLXNtIHB4LTIuNSBweS0xIHJvdW5kZWQgYm9yZGVyIGJvcmRlci1zbGF0ZS03MDAvODAgdGV4dC1bMTFweF0gZm9udC1tb25vIHRleHQtc2xhdGUtMzAwIGZsZXggaXRlbXMtY2VudGVyIGdhcC0yIHNoYWRvdy1sZ1wiPlxuICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJmb250LXNlbWlib2xkIHRleHQtc2xhdGUtNDAwXCI+MDwvc3Bhbj5cbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJ3LTIwIGgtMS41IGJnLWdyYWRpZW50LXRvLXIgZnJvbS13aGl0ZSB2aWEtc2xhdGUtNDAwIHRvLXdoaXRlIGJvcmRlci10IGJvcmRlci1iIGJvcmRlci1zbGF0ZS05MDAgcmVsYXRpdmVcIj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImFic29sdXRlIGxlZnQtMS8yIHRvcC0wIGJvdHRvbS0wIHctMC41IGJnLWJsYWNrXCIgLz5cbiAgICAgICAgPC9kaXY+XG4gICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cImZvbnQtYm9sZCB0ZXh0LXdoaXRlXCI+MTIga208L3NwYW4+XG4gICAgICA8L2Rpdj5cblxuICAgICAgey8qIFNlbGVjdGVkIERpc3RyaWN0IEluZm8gQ2FyZCAqL31cbiAgICAgIHtzZWxlY3RlZERpc3RyaWN0ICYmIChcbiAgICAgICAgPGRpdiBjbGFzc05hbWU9XCJhYnNvbHV0ZSBib3R0b20tNiByaWdodC02IHotWzEwMDBdIHctODAgYmctWyMwOTEwMWVdLzk1IGJhY2tkcm9wLWJsdXItbWQgYm9yZGVyIGJvcmRlci1za3ktNjAwIHJvdW5kZWQtbGcgcC0zIHRleHQtd2hpdGUgc2hhZG93LTJ4bCBzcGFjZS15LTJcIj5cbiAgICAgICAgICA8ZGl2IGNsYXNzTmFtZT1cImZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktYmV0d2VlbiBib3JkZXItYiBib3JkZXItc2xhdGUtNzAwIHBiLTEuNVwiPlxuICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1bMTBweF0gdGV4dC1za3ktNDAwIGZvbnQtYm9sZCB1cHBlcmNhc2VcIj5cbiAgICAgICAgICAgICAgICB7c2VsZWN0ZWREaXN0cmljdC5raW5kfVxuICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICAgIDxoMyBjbGFzc05hbWU9XCJ0ZXh0LXNtIGZvbnQtYm9sZCB0ZXh0LXdoaXRlXCI+e3NlbGVjdGVkRGlzdHJpY3QubmFtZX08L2gzPlxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICAgIG9uQ2xpY2s9eygpID0+IHNldFNlbGVjdGVkRGlzdHJpY3QobnVsbCl9XG4gICAgICAgICAgICAgIGNsYXNzTmFtZT1cInRleHQtc2xhdGUtNDAwIGhvdmVyOnRleHQtd2hpdGVcIlxuICAgICAgICAgICAgPlxuICAgICAgICAgICAgICA8WCBjbGFzc05hbWU9XCJ3LTQgaC00XCIgLz5cbiAgICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgIDxkaXYgY2xhc3NOYW1lPVwidGV4dC14cyBzcGFjZS15LTEgdGV4dC1zbGF0ZS0zMDBcIj5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtc2xhdGUtNDAwXCI+VsO5bmcgcGjDsm5nIHRo4bunOjwvc3Bhbj57JyAnfVxuICAgICAgICAgICAgICA8c3Ryb25nIGNsYXNzTmFtZT1cInRleHQtZW1lcmFsZC00MDBcIj57c2VsZWN0ZWREaXN0cmljdC5kZWZlbnNpdmVTZWN0b3J9PC9zdHJvbmc+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICAgIDxkaXY+XG4gICAgICAgICAgICAgIDxzcGFuIGNsYXNzTmFtZT1cInRleHQtc2xhdGUtNDAwXCI+RGnhu4duIHTDrWNoOjwvc3Bhbj57JyAnfVxuICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJmb250LW1vbm9cIj57c2VsZWN0ZWREaXN0cmljdC5hcmVhS20yfSBrbcKyPC9zcGFuPlxuICAgICAgICAgICAgPC9kaXY+XG4gICAgICAgICAgICA8ZGl2PlxuICAgICAgICAgICAgICA8c3BhbiBjbGFzc05hbWU9XCJ0ZXh0LXNsYXRlLTQwMFwiPkJhbiBDSFFTOjwvc3Bhbj57JyAnfVxuICAgICAgICAgICAgICA8c3Bhbj57c2VsZWN0ZWREaXN0cmljdC5taWxpdGFyeUNvbW1hbmRBZGRyZXNzfTwvc3Bhbj5cbiAgICAgICAgICAgIDwvZGl2PlxuICAgICAgICAgICAgPGRpdj5cbiAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwidGV4dC1zbGF0ZS00MDBcIj5T4buRIG3DoXkgdHLhu7FjIGJhbjo8L3NwYW4+eycgJ31cbiAgICAgICAgICAgICAgPHNwYW4gY2xhc3NOYW1lPVwiZm9udC1tb25vIHRleHQtZW1lcmFsZC0zMDAgZm9udC1zZW1pYm9sZFwiPlxuICAgICAgICAgICAgICAgIHtzZWxlY3RlZERpc3RyaWN0Lm1pbGl0YXJ5Q29tbWFuZFBob25lfVxuICAgICAgICAgICAgICA8L3NwYW4+XG4gICAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8L2Rpdj5cbiAgICAgICAgICA8YnV0dG9uXG4gICAgICAgICAgICBvbkNsaWNrPXsoKSA9PiB7XG4gICAgICAgICAgICAgIG9uU2V0QnVmZmVyQ29uZmlnKHtcbiAgICAgICAgICAgICAgICBhY3RpdmU6IHRydWUsXG4gICAgICAgICAgICAgICAgY2VudGVyTGF0OiBzZWxlY3RlZERpc3RyaWN0LmNlbnRlclswXSxcbiAgICAgICAgICAgICAgICBjZW50ZXJMbmc6IHNlbGVjdGVkRGlzdHJpY3QuY2VudGVyWzFdLFxuICAgICAgICAgICAgICAgIHJhZGl1c0ttOiAxMCxcbiAgICAgICAgICAgICAgICB0YXJnZXRVbml0TmFtZTogc2VsZWN0ZWREaXN0cmljdC5uYW1lLFxuICAgICAgICAgICAgICB9KTtcbiAgICAgICAgICAgIH19XG4gICAgICAgICAgICBjbGFzc05hbWU9XCJ3LWZ1bGwgbXQtMSBiZy1za3ktNzAwIGhvdmVyOmJnLXNreS02MDAgdGV4dC13aGl0ZSB0ZXh0LVsxMXB4XSBmb250LXNlbWlib2xkIHB5LTEuNSByb3VuZGVkIGZsZXggaXRlbXMtY2VudGVyIGp1c3RpZnktY2VudGVyIGdhcC0xLjVcIlxuICAgICAgICAgID5cbiAgICAgICAgICAgIDxDaXJjbGVEb3QgY2xhc3NOYW1lPVwidy0zLjUgaC0zLjVcIiAvPlxuICAgICAgICAgICAgPHNwYW4+VOG6oW8gdsO5bmcgxJHhu4dtIHBow7JuZyB0aOG7pyBxdeG6rW4gKDEwa20pPC9zcGFuPlxuICAgICAgICAgIDwvYnV0dG9uPlxuICAgICAgICA8L2Rpdj5cbiAgICAgICl9XG4gICAgPC9kaXY+XG4gICk7XG59O1xuIl19
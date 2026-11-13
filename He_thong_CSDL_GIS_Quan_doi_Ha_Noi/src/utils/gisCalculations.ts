import { HANOI_OUTER_POLYGON } from "/src/data/hanoiDistricts.ts";
import { calculateDistanceKm, toRadians, toDegrees } from "/src/utils/coordinateConversion.ts";
/**
* Check if a point (lat, lng) is inside a polygon using ray casting algorithm
*/
export function isPointInPolygon(point, polygon) {
	const [lat, lng] = point;
	let inside = false;
	for (let i = 0, j = polygon.length - 1; i < polygon.length; j = i++) {
		const [xi, yi] = polygon[i];
		const [xj, yj] = polygon[j];
		const intersect = yi > lng !== yj > lng && lat < (xj - xi) * (lng - yi) / (yj - yi) + xi;
		if (intersect) inside = !inside;
	}
	return inside;
}
/**
* Generate circle polygon points for buffer zone display in Leaflet (in [lat, lng])
*/
export function createBufferPolygon(centerLat, centerLng, radiusKm, pointsCount = 64) {
	const points = [];
	const earthRadiusKm = 6371;
	const latRad = toRadians(centerLat);
	const lngRad = toRadians(centerLng);
	const distRad = radiusKm / earthRadiusKm;
	for (let i = 0; i <= pointsCount; i++) {
		const bearing = 2 * Math.PI * i / pointsCount;
		const ptLatRad = Math.asin(Math.sin(latRad) * Math.cos(distRad) + Math.cos(latRad) * Math.sin(distRad) * Math.cos(bearing));
		const ptLngRad = lngRad + Math.atan2(Math.sin(bearing) * Math.sin(distRad) * Math.cos(latRad), Math.cos(distRad) - Math.sin(latRad) * Math.sin(ptLatRad));
		points.push([toDegrees(ptLatRad), toDegrees(ptLngRad)]);
	}
	return points;
}
/**
* Find K-Nearest Units from a target location
*/
export function findNearestUnits(targetLat, targetLng, units, k = 5) {
	return units.map((unit) => ({
		unit,
		distanceKm: calculateDistanceKm(targetLat, targetLng, unit.latitude, unit.longitude)
	})).sort((a, b) => a.distanceKm - b.distanceKm).slice(0, k);
}
/**
* GIS Topology & Quality Check for military database
*/
export function auditMilitaryData(units) {
	const issues = [];
	const codeMap = new Map();
	const coordMap = new Map();
	units.forEach((unit) => {
		// 1. Boundary check: Coordinates inside Hanoi bounding box & outer polygon
		const inHanoiBox = unit.latitude >= 20.55 && unit.latitude <= 21.4 && unit.longitude >= 105.25 && unit.longitude <= 106.1;
		const inHanoiPoly = isPointInPolygon([unit.latitude, unit.longitude], HANOI_OUTER_POLYGON);
		if (!inHanoiBox || !inHanoiPoly) {
			issues.push({
				id: `err_bound_${unit.id}`,
				unitId: unit.id,
				unitCode: unit.code,
				unitName: unit.name,
				type: "outside_hanoi",
				severity: "error",
				field: "Tọa độ không gian (Lat/Lng)",
				description: `Tọa độ (${unit.latitude.toFixed(4)}, ${unit.longitude.toFixed(4)}) nằm ngoài ranh giới hành chính TP Hà Nội.`,
				suggestion: "Kiểm tra lại kinh độ, vĩ độ hoặc hệ quy chiếu VN-2000."
			});
		}
		// 2. Duplicate Code check
		if (codeMap.has(unit.code)) {
			issues.push({
				id: `err_code_${unit.id}`,
				unitId: unit.id,
				unitCode: unit.code,
				unitName: unit.name,
				type: "duplicate_code",
				severity: "error",
				field: "Mã định danh (code)",
				description: `Mã quân sự "${unit.code}" bị trùng lặp với đơn vị "${codeMap.get(unit.code)}".`,
				suggestion: "Mã định danh quân sự phải là duy nhất (Primary Key)."
			});
		} else {
			codeMap.set(unit.code, unit.name);
		}
		// 3. Duplicate coordinates check (within 10 meters)
		const coordKey = `${unit.latitude.toFixed(4)}_${unit.longitude.toFixed(4)}`;
		if (coordMap.has(coordKey)) {
			issues.push({
				id: `err_coord_${unit.id}`,
				unitId: unit.id,
				unitCode: unit.code,
				unitName: unit.name,
				type: "duplicate_coords",
				severity: "warning",
				field: "Tọa độ trùng",
				description: `Tọa độ trùng khớp với đơn vị "${coordMap.get(coordKey)}". Cần kiểm tra xem có đóng quân chung doanh trại không.`,
				suggestion: "Xác thực lại vị trí đóng quân hoặc chỉ rõ phân khu đóng quân."
			});
		} else {
			coordMap.set(coordKey, unit.name);
		}
		// 4. Missing critical attributes
		if (!unit.commander || unit.commander.trim() === "") {
			issues.push({
				id: `err_cmd_${unit.id}`,
				unitId: unit.id,
				unitCode: unit.code,
				unitName: unit.name,
				type: "missing_fields",
				severity: "warning",
				field: "Chỉ huy trưởng / Giám đốc",
				description: "Chưa cập nhật thông tin Chỉ huy trưởng.",
				suggestion: "Bổ sung họ tên và cấp bậc sĩ quan chỉ huy."
			});
		}
		if (!unit.phone || unit.phone.trim() === "") {
			issues.push({
				id: `err_phone_${unit.id}`,
				unitId: unit.id,
				unitCode: unit.code,
				unitName: unit.name,
				type: "missing_fields",
				severity: "warning",
				field: "Số điện thoại trực ban",
				description: "Thiếu số điện thoại trực ban tác chiến khẩn cấp.",
				suggestion: "Bổ sung số máy quân sự hoặc đường dây nóng bảo đảm thông tin."
			});
		}
	});
	return issues;
}
/**
* Generate GeoJSON FeatureCollection
*/
export function unitsToGeoJSON(units) {
	const featureCollection = {
		type: "FeatureCollection",
		name: "MilitaryUnits_Hanoi_EPSG4326",
		crs: {
			type: "name",
			properties: { name: "urn:ogc:def:crs:OGC:1.3:CRS84" }
		},
		features: units.map((u) => ({
			type: "Feature",
			geometry: {
				type: "Point",
				coordinates: [u.longitude, u.latitude]
			},
			properties: {
				id: u.id,
				code: u.code,
				name: u.name,
				shortName: u.shortName,
				unitType: u.unitType,
				branch: u.branch,
				parentUnit: u.parentUnit,
				district: u.district,
				ward: u.ward,
				address: u.address,
				x_vn2000: u.x_vn2000,
				y_vn2000: u.y_vn2000,
				commander: u.commander,
				phone: u.phone,
				securityLevel: u.securityLevel,
				readinessStatus: u.readinessStatus,
				personnelScale: u.personnelScale,
				establishedYear: u.establishedYear
			}
		}))
	};
	return JSON.stringify(featureCollection, null, 2);
}

//# sourceMappingURL=data:application/json;base64,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Q0FBQztDQUVELE9BQU87QUFDVDs7OztBQUtBLE9BQU8sU0FBUyxlQUFlLE9BQStCO0NBQzVELE1BQU0sb0JBQW9CO0VBQ3hCLE1BQU07RUFDTixNQUFNO0VBQ04sS0FBSztHQUNILE1BQU07R0FDTixZQUFZLEVBQ1YsTUFBTSxnQ0FDUjtFQUNGO0VBQ0EsVUFBVSxNQUFNLEtBQUssT0FBTztHQUMxQixNQUFNO0dBQ04sVUFBVTtJQUNSLE1BQU07SUFDTixhQUFhLENBQUMsRUFBRSxXQUFXLEVBQUUsUUFBUTtHQUN2QztHQUNBLFlBQVk7SUFDVixJQUFJLEVBQUU7SUFDTixNQUFNLEVBQUU7SUFDUixNQUFNLEVBQUU7SUFDUixXQUFXLEVBQUU7SUFDYixVQUFVLEVBQUU7SUFDWixRQUFRLEVBQUU7SUFDVixZQUFZLEVBQUU7SUFDZCxVQUFVLEVBQUU7SUFDWixNQUFNLEVBQUU7SUFDUixTQUFTLEVBQUU7SUFDWCxVQUFVLEVBQUU7SUFDWixVQUFVLEVBQUU7SUFDWixXQUFXLEVBQUU7SUFDYixPQUFPLEVBQUU7SUFDVCxlQUFlLEVBQUU7SUFDakIsaUJBQWlCLEVBQUU7SUFDbkIsZ0JBQWdCLEVBQUU7SUFDbEIsaUJBQWlCLEVBQUU7R0FDckI7RUFDRixFQUFFO0NBQ0o7Q0FFQSxPQUFPLEtBQUssVUFBVSxtQkFBbUIsTUFBTSxDQUFDO0FBQ2xEIiwibmFtZXMiOltdLCJzb3VyY2VzIjpbImdpc0NhbGN1bGF0aW9ucy50cyJdLCJ2ZXJzaW9uIjozLCJzb3VyY2VzQ29udGVudCI6WyJpbXBvcnQgeyBNaWxpdGFyeVVuaXQsIFRvcG9sb2d5SXNzdWUgfSBmcm9tICcuLi90eXBlcy9naXMnO1xuaW1wb3J0IHsgSEFOT0lfT1VURVJfUE9MWUdPTiB9IGZyb20gJy4uL2RhdGEvaGFub2lEaXN0cmljdHMnO1xuaW1wb3J0IHsgY2FsY3VsYXRlRGlzdGFuY2VLbSwgdG9SYWRpYW5zLCB0b0RlZ3JlZXMgfSBmcm9tICcuL2Nvb3JkaW5hdGVDb252ZXJzaW9uJztcblxuLyoqXG4gKiBDaGVjayBpZiBhIHBvaW50IChsYXQsIGxuZykgaXMgaW5zaWRlIGEgcG9seWdvbiB1c2luZyByYXkgY2FzdGluZyBhbGdvcml0aG1cbiAqL1xuZXhwb3J0IGZ1bmN0aW9uIGlzUG9pbnRJblBvbHlnb24ocG9pbnQ6IFtudW1iZXIsIG51bWJlcl0sIHBvbHlnb246IFtudW1iZXIsIG51bWJlcl1bXSk6IGJvb2xlYW4ge1xuICBjb25zdCBbbGF0LCBsbmddID0gcG9pbnQ7XG4gIGxldCBpbnNpZGUgPSBmYWxzZTtcbiAgZm9yIChsZXQgaSA9IDAsIGogPSBwb2x5Z29uLmxlbmd0aCAtIDE7IGkgPCBwb2x5Z29uLmxlbmd0aDsgaiA9IGkrKykge1xuICAgIGNvbnN0IFt4aSwgeWldID0gcG9seWdvbltpXTtcbiAgICBjb25zdCBbeGosIHlqXSA9IHBvbHlnb25bal07XG5cbiAgICBjb25zdCBpbnRlcnNlY3QgPVxuICAgICAgeWkgPiBsbmcgIT09IHlqID4gbG5nICYmXG4gICAgICBsYXQgPCAoKHhqIC0geGkpICogKGxuZyAtIHlpKSkgLyAoeWogLSB5aSkgKyB4aTtcbiAgICBpZiAoaW50ZXJzZWN0KSBpbnNpZGUgPSAhaW5zaWRlO1xuICB9XG4gIHJldHVybiBpbnNpZGU7XG59XG5cbi8qKlxuICogR2VuZXJhdGUgY2lyY2xlIHBvbHlnb24gcG9pbnRzIGZvciBidWZmZXIgem9uZSBkaXNwbGF5IGluIExlYWZsZXQgKGluIFtsYXQsIGxuZ10pXG4gKi9cbmV4cG9ydCBmdW5jdGlvbiBjcmVhdGVCdWZmZXJQb2x5Z29uKFxuICBjZW50ZXJMYXQ6IG51bWJlcixcbiAgY2VudGVyTG5nOiBudW1iZXIsXG4gIHJhZGl1c0ttOiBudW1iZXIsXG4gIHBvaW50c0NvdW50OiBudW1iZXIgPSA2NFxuKTogW251bWJlciwgbnVtYmVyXVtdIHtcbiAgY29uc3QgcG9pbnRzOiBbbnVtYmVyLCBudW1iZXJdW10gPSBbXTtcbiAgY29uc3QgZWFydGhSYWRpdXNLbSA9IDYzNzE7XG5cbiAgY29uc3QgbGF0UmFkID0gdG9SYWRpYW5zKGNlbnRlckxhdCk7XG4gIGNvbnN0IGxuZ1JhZCA9IHRvUmFkaWFucyhjZW50ZXJMbmcpO1xuICBjb25zdCBkaXN0UmFkID0gcmFkaXVzS20gLyBlYXJ0aFJhZGl1c0ttO1xuXG4gIGZvciAobGV0IGkgPSAwOyBpIDw9IHBvaW50c0NvdW50OyBpKyspIHtcbiAgICBjb25zdCBiZWFyaW5nID0gKDIgKiBNYXRoLlBJICogaSkgLyBwb2ludHNDb3VudDtcbiAgICBjb25zdCBwdExhdFJhZCA9IE1hdGguYXNpbihcbiAgICAgIE1hdGguc2luKGxhdFJhZCkgKiBNYXRoLmNvcyhkaXN0UmFkKSArXG4gICAgICAgIE1hdGguY29zKGxhdFJhZCkgKiBNYXRoLnNpbihkaXN0UmFkKSAqIE1hdGguY29zKGJlYXJpbmcpXG4gICAgKTtcbiAgICBjb25zdCBwdExuZ1JhZCA9XG4gICAgICBsbmdSYWQgK1xuICAgICAgTWF0aC5hdGFuMihcbiAgICAgICAgTWF0aC5zaW4oYmVhcmluZykgKiBNYXRoLnNpbihkaXN0UmFkKSAqIE1hdGguY29zKGxhdFJhZCksXG4gICAgICAgIE1hdGguY29zKGRpc3RSYWQpIC0gTWF0aC5zaW4obGF0UmFkKSAqIE1hdGguc2luKHB0TGF0UmFkKVxuICAgICAgKTtcblxuICAgIHBvaW50cy5wdXNoKFt0b0RlZ3JlZXMocHRMYXRSYWQpLCB0b0RlZ3JlZXMocHRMbmdSYWQpXSk7XG4gIH1cblxuICByZXR1cm4gcG9pbnRzO1xufVxuXG4vKipcbiAqIEZpbmQgSy1OZWFyZXN0IFVuaXRzIGZyb20gYSB0YXJnZXQgbG9jYXRpb25cbiAqL1xuZXhwb3J0IGZ1bmN0aW9uIGZpbmROZWFyZXN0VW5pdHMoXG4gIHRhcmdldExhdDogbnVtYmVyLFxuICB0YXJnZXRMbmc6IG51bWJlcixcbiAgdW5pdHM6IE1pbGl0YXJ5VW5pdFtdLFxuICBrOiBudW1iZXIgPSA1XG4pOiBBcnJheTx7IHVuaXQ6IE1pbGl0YXJ5VW5pdDsgZGlzdGFuY2VLbTogbnVtYmVyIH0+IHtcbiAgcmV0dXJuIHVuaXRzXG4gICAgLm1hcCgodW5pdCkgPT4gKHtcbiAgICAgIHVuaXQsXG4gICAgICBkaXN0YW5jZUttOiBjYWxjdWxhdGVEaXN0YW5jZUttKHRhcmdldExhdCwgdGFyZ2V0TG5nLCB1bml0LmxhdGl0dWRlLCB1bml0LmxvbmdpdHVkZSksXG4gICAgfSkpXG4gICAgLnNvcnQoKGEsIGIpID0+IGEuZGlzdGFuY2VLbSAtIGIuZGlzdGFuY2VLbSlcbiAgICAuc2xpY2UoMCwgayk7XG59XG5cbi8qKlxuICogR0lTIFRvcG9sb2d5ICYgUXVhbGl0eSBDaGVjayBmb3IgbWlsaXRhcnkgZGF0YWJhc2VcbiAqL1xuZXhwb3J0IGZ1bmN0aW9uIGF1ZGl0TWlsaXRhcnlEYXRhKHVuaXRzOiBNaWxpdGFyeVVuaXRbXSk6IFRvcG9sb2d5SXNzdWVbXSB7XG4gIGNvbnN0IGlzc3VlczogVG9wb2xvZ3lJc3N1ZVtdID0gW107XG4gIGNvbnN0IGNvZGVNYXAgPSBuZXcgTWFwPHN0cmluZywgc3RyaW5nPigpO1xuICBjb25zdCBjb29yZE1hcCA9IG5ldyBNYXA8c3RyaW5nLCBzdHJpbmc+KCk7XG5cbiAgdW5pdHMuZm9yRWFjaCgodW5pdCkgPT4ge1xuICAgIC8vIDEuIEJvdW5kYXJ5IGNoZWNrOiBDb29yZGluYXRlcyBpbnNpZGUgSGFub2kgYm91bmRpbmcgYm94ICYgb3V0ZXIgcG9seWdvblxuICAgIGNvbnN0IGluSGFub2lCb3ggPVxuICAgICAgdW5pdC5sYXRpdHVkZSA+PSAyMC41NSAmJlxuICAgICAgdW5pdC5sYXRpdHVkZSA8PSAyMS40MCAmJlxuICAgICAgdW5pdC5sb25naXR1ZGUgPj0gMTA1LjI1ICYmXG4gICAgICB1bml0LmxvbmdpdHVkZSA8PSAxMDYuMTA7XG5cbiAgICBjb25zdCBpbkhhbm9pUG9seSA9IGlzUG9pbnRJblBvbHlnb24oW3VuaXQubGF0aXR1ZGUsIHVuaXQubG9uZ2l0dWRlXSwgSEFOT0lfT1VURVJfUE9MWUdPTik7XG5cbiAgICBpZiAoIWluSGFub2lCb3ggfHwgIWluSGFub2lQb2x5KSB7XG4gICAgICBpc3N1ZXMucHVzaCh7XG4gICAgICAgIGlkOiBgZXJyX2JvdW5kXyR7dW5pdC5pZH1gLFxuICAgICAgICB1bml0SWQ6IHVuaXQuaWQsXG4gICAgICAgIHVuaXRDb2RlOiB1bml0LmNvZGUsXG4gICAgICAgIHVuaXROYW1lOiB1bml0Lm5hbWUsXG4gICAgICAgIHR5cGU6ICdvdXRzaWRlX2hhbm9pJyxcbiAgICAgICAgc2V2ZXJpdHk6ICdlcnJvcicsXG4gICAgICAgIGZpZWxkOiAnVOG7jWEgxJHhu5kga2jDtG5nIGdpYW4gKExhdC9MbmcpJyxcbiAgICAgICAgZGVzY3JpcHRpb246IGBU4buNYSDEkeG7mSAoJHt1bml0LmxhdGl0dWRlLnRvRml4ZWQoNCl9LCAke3VuaXQubG9uZ2l0dWRlLnRvRml4ZWQoNCl9KSBu4bqxbSBuZ2/DoGkgcmFuaCBnaeG7m2kgaMOgbmggY2jDrW5oIFRQIEjDoCBO4buZaS5gLFxuICAgICAgICBzdWdnZXN0aW9uOiAnS2nhu4NtIHRyYSBs4bqhaSBraW5oIMSR4buZLCB2xKkgxJHhu5kgaG/hurdjIGjhu4cgcXV5IGNoaeG6v3UgVk4tMjAwMC4nLFxuICAgICAgfSk7XG4gICAgfVxuXG4gICAgLy8gMi4gRHVwbGljYXRlIENvZGUgY2hlY2tcbiAgICBpZiAoY29kZU1hcC5oYXModW5pdC5jb2RlKSkge1xuICAgICAgaXNzdWVzLnB1c2goe1xuICAgICAgICBpZDogYGVycl9jb2RlXyR7dW5pdC5pZH1gLFxuICAgICAgICB1bml0SWQ6IHVuaXQuaWQsXG4gICAgICAgIHVuaXRDb2RlOiB1bml0LmNvZGUsXG4gICAgICAgIHVuaXROYW1lOiB1bml0Lm5hbWUsXG4gICAgICAgIHR5cGU6ICdkdXBsaWNhdGVfY29kZScsXG4gICAgICAgIHNldmVyaXR5OiAnZXJyb3InLFxuICAgICAgICBmaWVsZDogJ03DoyDEkeG7i25oIGRhbmggKGNvZGUpJyxcbiAgICAgICAgZGVzY3JpcHRpb246IGBNw6MgcXXDom4gc+G7sSBcIiR7dW5pdC5jb2RlfVwiIGLhu4sgdHLDuW5nIGzhurdwIHbhu5tpIMSRxqFuIHbhu4sgXCIke2NvZGVNYXAuZ2V0KHVuaXQuY29kZSl9XCIuYCxcbiAgICAgICAgc3VnZ2VzdGlvbjogJ03DoyDEkeG7i25oIGRhbmggcXXDom4gc+G7sSBwaOG6o2kgbMOgIGR1eSBuaOG6pXQgKFByaW1hcnkgS2V5KS4nLFxuICAgICAgfSk7XG4gICAgfSBlbHNlIHtcbiAgICAgIGNvZGVNYXAuc2V0KHVuaXQuY29kZSwgdW5pdC5uYW1lKTtcbiAgICB9XG5cbiAgICAvLyAzLiBEdXBsaWNhdGUgY29vcmRpbmF0ZXMgY2hlY2sgKHdpdGhpbiAxMCBtZXRlcnMpXG4gICAgY29uc3QgY29vcmRLZXkgPSBgJHt1bml0LmxhdGl0dWRlLnRvRml4ZWQoNCl9XyR7dW5pdC5sb25naXR1ZGUudG9GaXhlZCg0KX1gO1xuICAgIGlmIChjb29yZE1hcC5oYXMoY29vcmRLZXkpKSB7XG4gICAgICBpc3N1ZXMucHVzaCh7XG4gICAgICAgIGlkOiBgZXJyX2Nvb3JkXyR7dW5pdC5pZH1gLFxuICAgICAgICB1bml0SWQ6IHVuaXQuaWQsXG4gICAgICAgIHVuaXRDb2RlOiB1bml0LmNvZGUsXG4gICAgICAgIHVuaXROYW1lOiB1bml0Lm5hbWUsXG4gICAgICAgIHR5cGU6ICdkdXBsaWNhdGVfY29vcmRzJyxcbiAgICAgICAgc2V2ZXJpdHk6ICd3YXJuaW5nJyxcbiAgICAgICAgZmllbGQ6ICdU4buNYSDEkeG7mSB0csO5bmcnLFxuICAgICAgICBkZXNjcmlwdGlvbjogYFThu41hIMSR4buZIHRyw7luZyBraOG7m3AgduG7m2kgxJHGoW4gduG7iyBcIiR7Y29vcmRNYXAuZ2V0KGNvb3JkS2V5KX1cIi4gQ+G6p24ga2nhu4NtIHRyYSB4ZW0gY8OzIMSRw7NuZyBxdcOibiBjaHVuZyBkb2FuaCB0cuG6oWkga2jDtG5nLmAsXG4gICAgICAgIHN1Z2dlc3Rpb246ICdYw6FjIHRo4buxYyBs4bqhaSB24buLIHRyw60gxJHDs25nIHF1w6JuIGhv4bq3YyBjaOG7iSByw7UgcGjDom4ga2h1IMSRw7NuZyBxdcOibi4nLFxuICAgICAgfSk7XG4gICAgfSBlbHNlIHtcbiAgICAgIGNvb3JkTWFwLnNldChjb29yZEtleSwgdW5pdC5uYW1lKTtcbiAgICB9XG5cbiAgICAvLyA0LiBNaXNzaW5nIGNyaXRpY2FsIGF0dHJpYnV0ZXNcbiAgICBpZiAoIXVuaXQuY29tbWFuZGVyIHx8IHVuaXQuY29tbWFuZGVyLnRyaW0oKSA9PT0gJycpIHtcbiAgICAgIGlzc3Vlcy5wdXNoKHtcbiAgICAgICAgaWQ6IGBlcnJfY21kXyR7dW5pdC5pZH1gLFxuICAgICAgICB1bml0SWQ6IHVuaXQuaWQsXG4gICAgICAgIHVuaXRDb2RlOiB1bml0LmNvZGUsXG4gICAgICAgIHVuaXROYW1lOiB1bml0Lm5hbWUsXG4gICAgICAgIHR5cGU6ICdtaXNzaW5nX2ZpZWxkcycsXG4gICAgICAgIHNldmVyaXR5OiAnd2FybmluZycsXG4gICAgICAgIGZpZWxkOiAnQ2jhu4kgaHV5IHRyxrDhu59uZyAvIEdpw6FtIMSR4buRYycsXG4gICAgICAgIGRlc2NyaXB0aW9uOiAnQ2jGsGEgY+G6rXAgbmjhuq10IHRow7RuZyB0aW4gQ2jhu4kgaHV5IHRyxrDhu59uZy4nLFxuICAgICAgICBzdWdnZXN0aW9uOiAnQuG7lSBzdW5nIGjhu40gdMOqbiB2w6AgY+G6pXAgYuG6rWMgc8SpIHF1YW4gY2jhu4kgaHV5LicsXG4gICAgICB9KTtcbiAgICB9XG5cbiAgICBpZiAoIXVuaXQucGhvbmUgfHwgdW5pdC5waG9uZS50cmltKCkgPT09ICcnKSB7XG4gICAgICBpc3N1ZXMucHVzaCh7XG4gICAgICAgIGlkOiBgZXJyX3Bob25lXyR7dW5pdC5pZH1gLFxuICAgICAgICB1bml0SWQ6IHVuaXQuaWQsXG4gICAgICAgIHVuaXRDb2RlOiB1bml0LmNvZGUsXG4gICAgICAgIHVuaXROYW1lOiB1bml0Lm5hbWUsXG4gICAgICAgIHR5cGU6ICdtaXNzaW5nX2ZpZWxkcycsXG4gICAgICAgIHNldmVyaXR5OiAnd2FybmluZycsXG4gICAgICAgIGZpZWxkOiAnU+G7kSDEkWnhu4duIHRob+G6oWkgdHLhu7FjIGJhbicsXG4gICAgICAgIGRlc2NyaXB0aW9uOiAnVGhp4bq/dSBz4buRIMSRaeG7h24gdGhv4bqhaSB0cuG7sWMgYmFuIHTDoWMgY2hp4bq/biBraOG6qW4gY+G6pXAuJyxcbiAgICAgICAgc3VnZ2VzdGlvbjogJ0Lhu5Ugc3VuZyBz4buRIG3DoXkgcXXDom4gc+G7sSBob+G6t2MgxJHGsOG7nW5nIGTDonkgbsOzbmcgYuG6o28gxJHhuqNtIHRow7RuZyB0aW4uJyxcbiAgICAgIH0pO1xuICAgIH1cbiAgfSk7XG5cbiAgcmV0dXJuIGlzc3Vlcztcbn1cblxuLyoqXG4gKiBHZW5lcmF0ZSBHZW9KU09OIEZlYXR1cmVDb2xsZWN0aW9uXG4gKi9cbmV4cG9ydCBmdW5jdGlvbiB1bml0c1RvR2VvSlNPTih1bml0czogTWlsaXRhcnlVbml0W10pOiBzdHJpbmcge1xuICBjb25zdCBmZWF0dXJlQ29sbGVjdGlvbiA9IHtcbiAgICB0eXBlOiAnRmVhdHVyZUNvbGxlY3Rpb24nLFxuICAgIG5hbWU6ICdNaWxpdGFyeVVuaXRzX0hhbm9pX0VQU0c0MzI2JyxcbiAgICBjcnM6IHtcbiAgICAgIHR5cGU6ICduYW1lJyxcbiAgICAgIHByb3BlcnRpZXM6IHtcbiAgICAgICAgbmFtZTogJ3VybjpvZ2M6ZGVmOmNyczpPR0M6MS4zOkNSUzg0JyxcbiAgICAgIH0sXG4gICAgfSxcbiAgICBmZWF0dXJlczogdW5pdHMubWFwKCh1KSA9PiAoe1xuICAgICAgdHlwZTogJ0ZlYXR1cmUnLFxuICAgICAgZ2VvbWV0cnk6IHtcbiAgICAgICAgdHlwZTogJ1BvaW50JyxcbiAgICAgICAgY29vcmRpbmF0ZXM6IFt1LmxvbmdpdHVkZSwgdS5sYXRpdHVkZV0sXG4gICAgICB9LFxuICAgICAgcHJvcGVydGllczoge1xuICAgICAgICBpZDogdS5pZCxcbiAgICAgICAgY29kZTogdS5jb2RlLFxuICAgICAgICBuYW1lOiB1Lm5hbWUsXG4gICAgICAgIHNob3J0TmFtZTogdS5zaG9ydE5hbWUsXG4gICAgICAgIHVuaXRUeXBlOiB1LnVuaXRUeXBlLFxuICAgICAgICBicmFuY2g6IHUuYnJhbmNoLFxuICAgICAgICBwYXJlbnRVbml0OiB1LnBhcmVudFVuaXQsXG4gICAgICAgIGRpc3RyaWN0OiB1LmRpc3RyaWN0LFxuICAgICAgICB3YXJkOiB1LndhcmQsXG4gICAgICAgIGFkZHJlc3M6IHUuYWRkcmVzcyxcbiAgICAgICAgeF92bjIwMDA6IHUueF92bjIwMDAsXG4gICAgICAgIHlfdm4yMDAwOiB1Lnlfdm4yMDAwLFxuICAgICAgICBjb21tYW5kZXI6IHUuY29tbWFuZGVyLFxuICAgICAgICBwaG9uZTogdS5waG9uZSxcbiAgICAgICAgc2VjdXJpdHlMZXZlbDogdS5zZWN1cml0eUxldmVsLFxuICAgICAgICByZWFkaW5lc3NTdGF0dXM6IHUucmVhZGluZXNzU3RhdHVzLFxuICAgICAgICBwZXJzb25uZWxTY2FsZTogdS5wZXJzb25uZWxTY2FsZSxcbiAgICAgICAgZXN0YWJsaXNoZWRZZWFyOiB1LmVzdGFibGlzaGVkWWVhcixcbiAgICAgIH0sXG4gICAgfSkpLFxuICB9O1xuXG4gIHJldHVybiBKU09OLnN0cmluZ2lmeShmZWF0dXJlQ29sbGVjdGlvbiwgbnVsbCwgMik7XG59XG4iXX0=
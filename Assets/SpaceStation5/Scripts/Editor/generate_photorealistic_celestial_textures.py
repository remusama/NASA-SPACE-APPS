import numpy as np
from PIL import Image, ImageDraw, ImageFilter
import os
import math

textures_dir = r"d:\MY proyects\Nasa spaces Apps\Assets\SpaceStation5\Textures"
os.makedirs(textures_dir, exist_ok=True)

# -------------------------------------------------------------
# 1. GENERATE PHOTOREALISTIC DEEP SPACE STARFIELD (4096 x 2048)
# -------------------------------------------------------------
def generate_photorealistic_sky():
    print("Generating photorealistic astronomical starfield (4096x2048)...")
    width, height = 4096, 2048
    
    # 1. Base deep space vacuum (pitch black with microscopic blue cosmic glow)
    np.random.seed(1337)
    x = np.linspace(-math.pi, math.pi, width)
    y = np.linspace(-math.pi/2, math.pi/2, height)
    xx, yy = np.meshgrid(x, y)
    
    # Milky Way galactic belt: Smooth tilted great circle across the sky
    # Inclination ~ 35 degrees
    gal_lat = yy * math.cos(0.55) - xx * math.sin(0.55)
    gal_dist = np.abs(gal_lat)
    
    # Galactic plane mask: Dense near the equator, soft Gaussian falloff
    mw_belt = np.exp(-(gal_dist**2) / 0.12)
    
    # Galactic Core Bulge (centered around Sagittarius)
    core_x = xx - 0.4
    core_y = yy - 0.2
    core_dist = np.sqrt(core_x**2 * 1.8 + core_y**2 * 4.5)
    core_glow = np.exp(-core_dist * 2.8) * 1.4
    
    # Fractal interstellar dust lanes (dark absorption clouds inside the Milky Way)
    dust1 = np.sin(xx * 6.0 + np.sin(yy * 10.0)) * 0.5 + 0.5
    dust2 = np.sin(xx * 14.0 - yy * 8.0) * 0.5 + 0.5
    dust_lanes = np.clip(1.0 - (dust1 * 0.5 + dust2 * 0.5) * mw_belt * 1.4, 0.15, 1.0)
    
    # Subtle, organic gas filaments (NO circular blobs!)
    # Wisps of hydrogen-alpha (red/crimson) and oxygen-III (cyan)
    gas_wisp1 = np.sin(xx * 4.0 + np.cos(yy * 5.0)) * np.cos(yy * 3.5 + np.sin(xx * 3.0))
    gas_wisp2 = np.sin(xx * 8.0 - yy * 6.0) * np.sin(xx * 5.0 + yy * 7.0)
    wisps = np.clip((gas_wisp1 * 0.6 + gas_wisp2 * 0.4) * mw_belt * dust_lanes, 0.0, 1.0)
    
    # Color composition:
    # Milky Way star dust: Warm golden-white at core, silver-white along the belt
    r_val = mw_belt * 22.0 + core_glow * 65.0 + wisps * 18.0
    g_val = mw_belt * 20.0 + core_glow * 48.0 + wisps * 12.0
    b_val = mw_belt * 28.0 + core_glow * 35.0 + wisps * 24.0
    
    # Apply dust lane extinction
    r_val *= dust_lanes
    g_val *= dust_lanes
    b_val *= dust_lanes
    
    # Base background: Deep cosmic darkness
    r_val += 2.0
    g_val += 2.5
    b_val += 4.0
    
    bg_rgba = np.zeros((height, width, 4), dtype=np.uint8)
    bg_rgba[:, :, 0] = np.clip(r_val, 0, 255).astype(np.uint8)
    bg_rgba[:, :, 1] = np.clip(g_val, 0, 255).astype(np.uint8)
    bg_rgba[:, :, 2] = np.clip(b_val, 0, 255).astype(np.uint8)
    bg_rgba[:, :, 3] = 255
    
    sky_img = Image.fromarray(bg_rgba, "RGBA")
    draw = ImageDraw.Draw(sky_img)
    
    # 2. Render 25,000 Astronomical Pinpoint Stars
    print("  Rendering 25,000 astronomical stars...")
    # Background faint stars (magnitude 6 - 12)
    num_faint = 20000
    fx = np.random.randint(0, width, num_faint)
    fy = np.random.randint(0, height, num_faint)
    fb = np.random.uniform(0.15, 0.85, num_faint)
    
    # Milky Way density boost: stars are 4x denser along the galactic plane
    mw_density = np.exp(-((fy / height - 0.5 - 0.15 * np.sin(fx / width * math.pi * 2.0))**2) / 0.03)
    
    for i in range(num_faint):
        px, py, b = fx[i], fy[i], fb[i]
        # Skip some stars outside the milky way for realistic clumpiness
        if mw_density[i] < 0.2 and np.random.rand() > 0.65:
            continue
        brightness = int(b * 220)
        c_r = brightness
        c_g = brightness
        c_b = min(255, int(brightness * 1.1))
        draw.point((px, py), fill=(c_r, c_g, c_b, 255))
        
    # Prominent spectral stars (2,500 stars with real stellar color spectra)
    num_prom = 2500
    px = np.random.randint(0, width, num_prom)
    py = np.random.randint(0, height, num_prom)
    star_spectra = [
        (185, 215, 255), # O/B Blue-white supergiant
        (230, 240, 255), # A White main sequence (Sirius, Vega)
        (255, 250, 230), # F Yellow-white (Procyon)
        (255, 235, 175), # G Solar yellow (Sun, Alpha Centauri)
        (255, 185, 120), # K Orange giant (Arcturus, Aldebaran)
        (255, 110, 85),  # M Red supergiant (Betelgeuse, Antares)
    ]
    
    for i in range(num_prom):
        x_pos, y_pos = px[i], py[i]
        color = star_spectra[np.random.randint(len(star_spectra))]
        r = np.random.choice([1, 1, 1, 1, 2])
        draw.ellipse([x_pos - r, y_pos - r, x_pos + r, y_pos + r], fill=(color[0], color[1], color[2], 255))
        
    # 60 Brilliant 1st-magnitude stars with delicate optical diffraction spikes
    num_bright = 60
    bx = np.random.randint(40, width - 40, num_bright)
    by = np.random.randint(40, height - 40, num_bright)
    for i in range(num_bright):
        x_pos, y_pos = bx[i], by[i]
        color = star_spectra[np.random.randint(len(star_spectra))]
        r = np.random.randint(2, 4)
        # Core
        draw.ellipse([x_pos - r, y_pos - r, x_pos + r, y_pos + r], fill=(255, 255, 255, 255))
        draw.ellipse([x_pos - r - 1, y_pos - r - 1, x_pos + r + 1, y_pos + r + 1], 
                     fill=(color[0], color[1], color[2], 180))
        # Delicate diffraction spike
        spike = np.random.randint(6, 12)
        draw.line([x_pos - spike, y_pos, x_pos + spike, y_pos], fill=(color[0], color[1], color[2], 160), width=1)
        draw.line([x_pos, y_pos - spike, x_pos, y_pos + spike], fill=(color[0], color[1], color[2], 160), width=1)
        
    out_file = os.path.join(textures_dir, "T_Deep_Space_Stars.png")
    sky_img.save(out_file, "PNG", optimize=True)
    print("Saved photorealistic starfield to:", out_file)

# -------------------------------------------------------------
# 2. GENERATE PHOTOREALISTIC EARTH ALBEDO, CITY LIGHTS & CLOUDS
# -------------------------------------------------------------
def generate_photorealistic_earth():
    print("Generating photorealistic Earth textures (2048x1024)...")
    width, height = 2048, 1024
    
    # 1. Earth Daytime Albedo
    # Rich deep sapphire oceans, realistic continents, lush greens, desert sands, polar ice
    x = np.linspace(-math.pi, math.pi, width)
    y = np.linspace(-math.pi/2, math.pi/2, height)
    xx, yy = np.meshgrid(x, y)
    
    # Continental landmass shapes (multi-frequency Perlin approximation)
    land1 = (np.sin(xx * 2.5 + 0.4) * np.cos(yy * 2.2)) * 0.5 + 0.5
    land2 = (np.sin(xx * 5.0 - 1.2) * np.cos(yy * 4.5 + 0.3)) * 0.5 + 0.5
    land3 = (np.sin(xx * 9.0 + yy * 8.0)) * 0.5 + 0.5
    land_height = land1 * 0.6 + land2 * 0.3 + land3 * 0.1
    
    # Ocean vs Land threshold
    is_land = land_height > 0.48
    
    # Polar ice caps (North & South poles)
    is_north_ice = yy > 1.25
    is_south_ice = yy < -1.15
    is_ice = is_north_ice | is_south_ice
    
    # Oceans: Deep sapphire blue gradient with continental shelf teal
    ocean_depth = np.clip((0.48 - land_height) / 0.48, 0.0, 1.0)
    ocean_r = 8 + 12 * (1.0 - ocean_depth)
    ocean_g = 24 + 48 * (1.0 - ocean_depth)
    ocean_b = 68 + 75 * (1.0 - ocean_depth)
    
    # Land terrain colors (desert amber/gold in tropics, temperate forest green, mountain grey)
    lat_abs = np.abs(yy)
    is_desert = (lat_abs > 0.25) & (lat_abs < 0.65) & (land_height < 0.62)
    
    land_r = np.where(is_desert, 185.0 + 35 * land2, 45.0 + 30 * land2)
    land_g = np.where(is_desert, 150.0 + 25 * land2, 85.0 + 40 * land2)
    land_b = np.where(is_desert, 90.0 + 20 * land2, 35.0 + 20 * land2)
    
    # Mountain ranges
    is_mountain = land_height > 0.72
    land_r = np.where(is_mountain, 120.0 + 40 * land3, land_r)
    land_g = np.where(is_mountain, 115.0 + 35 * land3, land_g)
    land_b = np.where(is_mountain, 110.0 + 30 * land3, land_b)
    
    # Apply to base image
    earth_r = np.where(is_land, land_r, ocean_r)
    earth_g = np.where(is_land, land_g, ocean_g)
    earth_b = np.where(is_land, land_b, ocean_b)
    
    # Snow & Ice Caps
    earth_r = np.where(is_ice, 240.0, earth_r)
    earth_g = np.where(is_ice, 245.0, earth_g)
    earth_b = np.where(is_ice, 252.0, earth_b)
    
    earth_rgb = np.zeros((height, width, 3), dtype=np.uint8)
    earth_rgb[:, :, 0] = np.clip(earth_r, 0, 255).astype(np.uint8)
    earth_rgb[:, :, 1] = np.clip(earth_g, 0, 255).astype(np.uint8)
    earth_rgb[:, :, 2] = np.clip(earth_b, 0, 255).astype(np.uint8)
    
    earth_img = Image.fromarray(earth_rgb, "RGB")
    earth_img = earth_img.filter(ImageFilter.GaussianBlur(radius=0.8))
    albedo_file = os.path.join(textures_dir, "T_Planet_Earth_Albedo.png")
    earth_img.save(albedo_file, "PNG", optimize=True)
    print("Saved Earth Day Albedo to:", albedo_file)
    
    # 2. Earth Night City Lights
    # Dense golden metropolitan webs along coastlines and river valleys
    np.random.seed(999)
    noise1 = np.random.rand(height, width)
    noise2 = np.random.rand(height, width)
    
    # Coastlines and temperate lands have highest city densities
    lat_weight = np.exp(-((yy - 0.6)**2) / 0.18) + 0.7 * np.exp(-((yy - 0.25)**2) / 0.15) + 0.4 * np.exp(-((yy + 0.4)**2) / 0.2)
    coast_mask = is_land & ~is_ice
    city_intensity = coast_mask.astype(float) * lat_weight * (noise1 > 0.86).astype(float)
    
    # Golden amber city light color: #FFE082 / #FFA726
    c_r = np.clip(city_intensity * 255 * (0.85 + 0.15 * noise2), 0, 255)
    c_g = np.clip(city_intensity * 195 * (0.85 + 0.15 * noise2), 0, 255)
    c_b = np.clip(city_intensity * 110 * (0.85 + 0.15 * noise2), 0, 255)
    
    lights_sharp = Image.fromarray(np.stack([c_r, c_g, c_b], axis=2).astype(np.uint8), "RGB")
    lights_glow = lights_sharp.filter(ImageFilter.GaussianBlur(radius=1.8))
    
    final_lights = np.array(lights_sharp).astype(float) * 1.6 + np.array(lights_glow).astype(float) * 0.7
    final_lights = np.clip(final_lights, 0, 255).astype(np.uint8)
    
    alpha = np.max(final_lights, axis=2)
    alpha = np.clip(alpha * 2.2, 0, 255).astype(np.uint8)
    
    lights_rgba = np.dstack([final_lights, alpha])
    lights_img = Image.fromarray(lights_rgba, "RGBA")
    night_file = os.path.join(textures_dir, "T_Planet_Earth_NightLights.png")
    lights_img.save(night_file, "PNG", optimize=True)
    print("Saved Earth Night Lights to:", night_file)
    
    # 3. Earth Dynamic Cloud Cover
    # Realistic swirling cyclone bands and weather fronts
    c1 = (np.sin(xx * 6.0 + np.sin(yy * 8.0)) * 0.5 + 0.5)
    c2 = (np.cos(xx * 12.0 - yy * 7.0) * 0.5 + 0.5)
    c3 = (np.sin(xx * 22.0 + yy * 18.0) * 0.5 + 0.5)
    clouds_dense = c1 * 0.5 + c2 * 0.35 + c3 * 0.15
    
    # Tropical convergence & storm belts
    storm_belt = np.exp(-((yy - 0.1)**2) / 0.08) * 0.4 + np.exp(-((yy - 0.75)**2) / 0.12) * 0.5 + np.exp(-((yy + 0.8)**2) / 0.12) * 0.5
    clouds_val = np.clip((clouds_dense + storm_belt - 0.42) * 2.4, 0.0, 1.0)
    
    # Cloud color: Brilliant white with subtle shading
    cr = np.clip(clouds_val * 255, 0, 255).astype(np.uint8)
    cg = np.clip(clouds_val * 255, 0, 255).astype(np.uint8)
    cb = np.clip(clouds_val * 255, 0, 255).astype(np.uint8)
    ca = np.clip(clouds_val * 235, 0, 235).astype(np.uint8)
    
    clouds_rgba = np.dstack([cr, cg, cb, ca])
    clouds_img = Image.fromarray(clouds_rgba, "RGBA")
    clouds_file = os.path.join(textures_dir, "T_Planet_Earth_Clouds.png")
    clouds_img.save(clouds_file, "PNG", optimize=True)
    print("Saved Earth Clouds to:", clouds_file)

# -------------------------------------------------------------
# 3. GENERATE PHOTOREALISTIC MOON (2048 x 1024)
# -------------------------------------------------------------
def generate_photorealistic_moon():
    print("Generating photorealistic Moon textures (2048x1024)...")
    width, height = 2048, 1024
    
    np.random.seed(333)
    x = np.linspace(-math.pi, math.pi, width)
    y = np.linspace(-math.pi/2, math.pi/2, height)
    xx, yy = np.meshgrid(x, y)
    
    # Lunar Maria: Basaltic dark lava plains
    m1 = np.exp(-((xx + 0.5)**2 * 2.5 + (yy - 0.3)**2 * 6.0)) # Imbrium
    m2 = np.exp(-((xx - 0.35)**2 * 3.5 + (yy - 0.08)**2 * 8.0)) # Tranquillitatis & Serenitatis
    m3 = np.exp(-((xx + 1.1)**2 * 2.0 + (yy - 0.0)**2 * 5.0)) # Oceanus Procellarum
    m4 = np.exp(-((xx - 1.05)**2 * 4.5 + (yy - 0.22)**2 * 9.0)) # Crisium
    maria = np.clip(m1 * 0.85 + m2 * 0.8 + m3 * 0.9 + m4 * 0.65, 0.0, 1.0)
    
    # Highlands regolith fine noise
    n1 = (np.sin(xx * 25.0 + np.cos(yy * 20.0)) * 0.5 + 0.5)
    n2 = (np.sin(xx * 60.0 - yy * 45.0) * 0.5 + 0.5)
    n3 = np.random.rand(height, width)
    
    lunar_base = 155.0 + 30.0 * n1 + 15.0 * n2 + 10.0 * n3
    # Darken basaltic maria
    lunar_base -= maria * 75.0
    
    moon_gray = np.clip(lunar_base, 35, 230).astype(np.uint8)
    moon_img = Image.fromarray(moon_gray, "L").convert("RGB")
    draw = ImageDraw.Draw(moon_img)
    
    # Major Rayed Impact Craters (Tycho with long ejecta rays, Copernicus, Kepler)
    major_craters = [
        (int(width * 0.48), int(height * 0.72), 22, 18), # Tycho
        (int(width * 0.42), int(height * 0.38), 16, 12), # Copernicus
        (int(width * 0.34), int(height * 0.42), 12, 8),  # Kepler
        (int(width * 0.54), int(height * 0.32), 14, 6),  # Plato / Aristoteles
    ]
    
    for cx, cy, cr, rays in major_craters:
        # Bright crater rim
        draw.ellipse([cx - cr, cy - cr, cx + cr, cy + cr], outline=(225, 225, 230), width=max(1, cr // 4))
        # Shadowed interior
        draw.ellipse([cx - cr + 1, cy - cr + 1, cx + cr - 1, cy + cr - 1], fill=(70, 70, 75))
        # Central peak
        draw.ellipse([cx - 2, cy - 2, cx + 2, cy + 2], fill=(240, 240, 245))
        # Radial ejecta rays
        for a in range(rays):
            angle = (a / rays) * 2 * math.pi + np.random.uniform(-0.08, 0.08)
            ray_len = np.random.randint(cr * 4, cr * 12)
            ex = int(cx + math.cos(angle) * ray_len)
            ey = int(cy + math.sin(angle) * ray_len)
            draw.line([cx, cy, ex, ey], fill=(215, 215, 220), width=1)
            
    # Small crater pockmarks (300 craters)
    for _ in range(300):
        cx = np.random.randint(10, width - 10)
        cy = np.random.randint(10, height - 10)
        cr = np.random.randint(2, 6)
        draw.ellipse([cx - cr, cy - cr, cx + cr, cy + cr], outline=(210, 210, 215), fill=(80, 80, 85))
        
    moon_img = moon_img.filter(ImageFilter.GaussianBlur(radius=0.6))
    moon_albedo = os.path.join(textures_dir, "T_Moon_Albedo.png")
    moon_img.save(moon_albedo, "PNG", optimize=True)
    print("Saved Moon Albedo to:", moon_albedo)
    
    # Generate Smooth Normal Map
    m_data = np.array(moon_img.convert("L")).astype(float)
    dzdx = np.gradient(m_data, axis=1) * 1.8
    dzdy = np.gradient(m_data, axis=0) * 1.8
    nx = -dzdx
    ny = -dzdy
    nz = 1.0
    mag = np.sqrt(nx**2 + ny**2 + nz**2)
    nx /= mag
    ny /= mag
    nz /= mag
    
    nr = np.clip((nx * 0.5 + 0.5) * 255, 0, 255).astype(np.uint8)
    ng = np.clip((ny * 0.5 + 0.5) * 255, 0, 255).astype(np.uint8)
    nb = np.clip((nz * 0.5 + 0.5) * 255, 0, 255).astype(np.uint8)
    
    norm_img = Image.fromarray(np.stack([nr, ng, nb], axis=2), "RGB")
    moon_normal = os.path.join(textures_dir, "T_Moon_Normal.png")
    norm_img.save(moon_normal, "PNG", optimize=True)
    print("Saved Moon Normal to:", moon_normal)

# -------------------------------------------------------------
# 4. GENERATE RADIANT SOLAR CORONA (1024 x 1024)
# -------------------------------------------------------------
def generate_photorealistic_sun():
    print("Generating Radiant Solar Corona (1024x1024)...")
    size = 1024
    cx, cy = size // 2, size // 2
    x = np.arange(size) - cx
    y = np.arange(size) - cy
    xx, yy = np.meshgrid(x, y)
    dist = np.sqrt(xx**2 + yy**2)
    angle = np.arctan2(yy, xx)
    
    # Pure incandescent white core (radius ~ 90)
    core_r = 90.0
    
    # Solar coronal streamers & flare rays
    rays1 = np.sin(angle * 10.0) * 0.5 + 0.5
    rays2 = np.sin(angle * 22.0 + 0.8) * 0.5 + 0.5
    rays3 = np.sin(angle * 38.0 - 1.4) * 0.5 + 0.5
    rays = rays1 * 0.45 + rays2 * 0.35 + rays3 * 0.20
    
    # Exponential radial glow falloff
    norm_dist = np.clip((dist - core_r) / (size * 0.45), 0.0, 1.0)
    corona_falloff = np.exp(-norm_dist * 3.8) * (0.65 + 0.65 * rays)
    
    # Anamorphic 4-point solar diffraction spikes
    beam_h = np.exp(-(yy**2) / 80.0) * np.exp(-np.abs(xx) / 260.0)
    beam_v = np.exp(-(xx**2) / 80.0) * np.exp(-np.abs(yy) / 260.0)
    diag_a = np.exp(-((xx + yy)**2) / 120.0) * np.exp(-dist / 300.0)
    diag_b = np.exp(-((xx - yy)**2) / 120.0) * np.exp(-dist / 300.0)
    beams = beam_h + beam_v + 0.5 * (diag_a + diag_b)
    
    total = np.clip(corona_falloff + beams * 0.8, 0.0, 1.0)
    
    # Incandescent white -> golden amber -> solar orange -> deep space
    sr = np.clip(total * 255 * 1.15, 0, 255).astype(np.uint8)
    sg = np.clip(total * 225 * 0.95, 0, 255).astype(np.uint8)
    sb = np.clip(total * 145 * 0.70, 0, 255).astype(np.uint8)
    sa = np.clip(total * 255 * 1.3, 0, 255).astype(np.uint8)
    
    # Core is 100% solid white
    inside = dist < core_r
    sr[inside] = 255
    sg[inside] = 255
    sb[inside] = 255
    sa[inside] = 255
    
    sun_rgba = np.dstack([sr, sg, sb, sa])
    sun_img = Image.fromarray(sun_rgba, "RGBA")
    sun_file = os.path.join(textures_dir, "T_Sun_Corona.png")
    sun_img.save(sun_file, "PNG", optimize=True)
    print("Saved Solar Corona to:", sun_file)

# -------------------------------------------------------------
# 5. GENERATE ACCRETION DISK PLASMA TEXTURE (2048 x 2048)
# -------------------------------------------------------------
def generate_accretion_disk_texture():
    print("Generating ultra-res Accretion Disk Plasma Texture (2048x2048)...")
    size = 2048
    cx, cy = size // 2, size // 2
    x = np.arange(size) - cx
    y = np.arange(size) - cy
    xx, yy = np.meshgrid(x, y)
    dist = np.sqrt(xx**2 + yy**2)
    angle = np.arctan2(yy, xx)
    
    # Event horizon shadow boundary
    r_shadow = size * 0.16
    r_outer = size * 0.48
    
    # Normalized radial coordinate (0 at shadow, 1 at outer edge)
    r_norm = np.clip((dist - r_shadow) / (r_outer - r_shadow), 0.0, 1.0)
    
    # Logarithmic vortex spiral arms
    spiral_angle = angle - np.log(np.maximum(dist, 10.0)) * 3.5
    plasma_arms1 = np.sin(spiral_angle * 3.0) * 0.5 + 0.5
    plasma_arms2 = np.sin(spiral_angle * 7.0 + 1.2) * 0.5 + 0.5
    plasma_noise = plasma_arms1 * 0.65 + plasma_arms2 * 0.35
    
    # Temperature gradient:
    # Inner = White-hot / electric blue
    # Mid = Blazing gold and intense solar orange
    # Outer = Deep interstellar crimson and dark dust filaments
    inner_mask = np.exp(-(r_norm**2) / 0.08)
    mid_mask = np.exp(-((r_norm - 0.35)**2) / 0.12)
    outer_mask = np.exp(-((r_norm - 0.75)**2) / 0.18)
    
    # Relativistic Doppler boosting (approaching side angle < 0 is brighter)
    doppler = np.cos(angle)
    doppler_boost = 1.0 + doppler * 0.55
    
    # Color channels
    pr = (inner_mask * 255 + mid_mask * 255 + outer_mask * 190) * (0.7 + 0.45 * plasma_noise) * doppler_boost
    pg = (inner_mask * 240 + mid_mask * 145 + outer_mask * 45) * (0.7 + 0.45 * plasma_noise) * doppler_boost
    pb = (inner_mask * 255 + mid_mask * 35 + outer_mask * 15) * (0.7 + 0.45 * plasma_noise) * doppler_boost
    
    # Alpha mask: 0 inside event horizon shadow, smooth rise, smooth fade at outer edge
    alpha = np.where(dist < r_shadow, 0.0, np.sin(r_norm * math.pi) * 255.0)
    alpha = np.clip(alpha * (0.75 + 0.35 * plasma_noise), 0, 255).astype(np.uint8)
    
    pr = np.clip(pr, 0, 255).astype(np.uint8)
    pg = np.clip(pg, 0, 255).astype(np.uint8)
    pb = np.clip(pb, 0, 255).astype(np.uint8)
    
    accretion_rgba = np.dstack([pr, pg, pb, alpha])
    acc_img = Image.fromarray(accretion_rgba, "RGBA")
    acc_file = os.path.join(textures_dir, "T_BlackHole_Accretion_Albedo.png")
    acc_img.save(acc_file, "PNG", optimize=True)
    print("Saved Accretion Disk Texture to:", acc_file)

if __name__ == "__main__":
    generate_photorealistic_sky()
    generate_photorealistic_earth()
    generate_photorealistic_moon()
    generate_photorealistic_sun()
    generate_accretion_disk_texture()
    print("All photorealistic celestial textures generated with complete success!")

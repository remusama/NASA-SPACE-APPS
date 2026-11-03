import numpy as np
from PIL import Image, ImageDraw, ImageFilter
import os
import math

textures_dir = r"d:\MY proyects\Nasa spaces Apps\Assets\SpaceStation5\Textures"
os.makedirs(textures_dir, exist_ok=True)

# -------------------------------------------------------------
# 1. GENERATE DEEP SPACE STARFIELD (4096 x 2048)
# -------------------------------------------------------------
def generate_deep_space_stars():
    print("Generating ultra-res deep space starfield (4096x2048)...")
    width, height = 4096, 2048
    img = Image.new("RGBA", (width, height), (3, 4, 8, 255))
    draw = ImageDraw.Draw(img)
    
    np.random.seed(42)
    
    # 1. Cosmic Background Radiation & Galactic Nebula Plane
    # Create smooth nebula layers using numpy
    x = np.linspace(-math.pi, math.pi, width)
    y = np.linspace(-math.pi/2, math.pi/2, height)
    xx, yy = np.meshgrid(x, y)
    
    # Galactic plane angle (tilted across the sky)
    tilt = 0.35
    gal_dist = np.abs(yy - tilt * np.sin(xx + 0.5))
    
    # Galactic core bulge
    core_dist = np.sqrt((xx - 0.2)**2 * 1.5 + (yy - 0.1)**2 * 6.0)
    core_mask = np.exp(-core_dist * 2.5)
    
    # Milky way band mask
    mw_mask = np.exp(-gal_dist * 4.0) * (0.6 + 0.4 * np.cos(xx * 2.0))
    
    # Nebula gas clouds (Perlin-like multi-frequency sinusoids)
    gas1 = (np.sin(xx * 3.5 + np.cos(yy * 4.0)) * 0.5 + 0.5) * (np.cos(yy * 3.0 + np.sin(xx * 2.5)) * 0.5 + 0.5)
    gas2 = (np.sin(xx * 7.0 - yy * 5.0) * 0.5 + 0.5) * (np.cos(xx * 4.0 + yy * 8.0) * 0.5 + 0.5)
    gas3 = (np.sin(xx * 12.0 + yy * 9.0) * 0.5 + 0.5)
    
    nebula = gas1 * 0.5 + gas2 * 0.35 + gas3 * 0.15
    
    # Base cosmic colors
    base_r = np.clip(10 * mw_mask + 45 * core_mask + 18 * nebula * mw_mask, 0, 255)
    base_g = np.clip(14 * mw_mask + 30 * core_mask + 12 * nebula * mw_mask, 0, 255)
    base_b = np.clip(28 * mw_mask + 25 * core_mask + 35 * nebula * mw_mask, 0, 255)
    
    # Add vibrant glowing emission nebula pockets (Magenta/Pink & Cyan/Teal)
    neb_pocket1 = np.exp(-((xx + 1.2)**2 * 2.5 + (yy - 0.2)**2 * 5.0)) * gas2
    neb_pocket2 = np.exp(-((xx - 1.8)**2 * 3.0 + (yy + 0.3)**2 * 6.0)) * gas1
    neb_pocket3 = np.exp(-((xx - 0.5)**2 * 2.0 + (yy - 0.4)**2 * 4.0)) * gas2
    
    base_r += neb_pocket1 * 85 + neb_pocket2 * 20 + neb_pocket3 * 75
    base_g += neb_pocket1 * 25 + neb_pocket2 * 75 + neb_pocket3 * 35
    base_b += neb_pocket1 * 95 + neb_pocket2 * 90 + neb_pocket3 * 110
    
    # Dark cosmic dust absorption lanes inside the milky way
    dust = (np.sin(xx * 5.0 + np.sin(yy * 8.0)) * 0.5 + 0.5) * mw_mask
    dust_mask = np.clip(1.0 - dust * 0.65, 0.2, 1.0)
    base_r *= dust_mask
    base_g *= dust_mask
    base_b *= dust_mask
    
    neb_array = np.zeros((height, width, 4), dtype=np.uint8)
    neb_array[:, :, 0] = np.clip(base_r + 3, 0, 255).astype(np.uint8)
    neb_array[:, :, 1] = np.clip(base_g + 4, 0, 255).astype(np.uint8)
    neb_array[:, :, 2] = np.clip(base_b + 8, 0, 255).astype(np.uint8)
    neb_array[:, :, 3] = 255
    
    img = Image.fromarray(neb_array, "RGBA")
    draw = ImageDraw.Draw(img)
    
    # 2. Add Dense Pinpoint Stars (12,000 stars)
    print("  Rendering 12,000 pinpoint stars...")
    # Background faint stars
    num_faint = 10000
    fx = np.random.randint(0, width, num_faint)
    fy = np.random.randint(0, height, num_faint)
    fb = np.random.uniform(0.3, 0.9, num_faint)
    # Concentration toward galactic plane
    for i in range(num_faint):
        px, py, b = fx[i], fy[i], fb[i]
        c = int(b * 220)
        draw.point((px, py), fill=(c, c, min(255, int(c * 1.15)), 255))
        
    # Prominent colored stars (1,500 stars)
    num_prom = 1500
    px = np.random.randint(0, width, num_prom)
    py = np.random.randint(0, height, num_prom)
    star_types = [
        (200, 225, 255), # Hot Blue/White O-B star
        (255, 255, 255), # Crisp White A-F star
        (255, 240, 200), # Warm Yellow G star (Solar type)
        (255, 190, 140), # Orange K giant
        (255, 120, 100), # Red M supergiant
    ]
    for i in range(num_prom):
        x_pos, y_pos = px[i], py[i]
        st = star_types[np.random.randint(len(star_types))]
        radius = np.random.choice([1, 1, 1, 2])
        draw.ellipse([x_pos - radius, y_pos - radius, x_pos + radius, y_pos + radius], 
                     fill=(st[0], st[1], st[2], 255))
        
    # Bright 1st-magnitude stars with subtle optical diffraction spikes (80 stars)
    num_bright = 80
    bx = np.random.randint(50, width - 50, num_bright)
    by = np.random.randint(50, height - 50, num_bright)
    for i in range(num_bright):
        x_pos, y_pos = bx[i], by[i]
        st = star_types[np.random.randint(len(star_types))]
        r = np.random.randint(2, 4)
        # Central glow
        draw.ellipse([x_pos - r, y_pos - r, x_pos + r, y_pos + r], fill=(255, 255, 255, 255))
        draw.ellipse([x_pos - r - 1, y_pos - r - 1, x_pos + r + 1, y_pos + r + 1], 
                     fill=(st[0], st[1], st[2], 160))
        # 4-point subtle spike
        spike_len = np.random.randint(6, 14)
        draw.line([x_pos - spike_len, y_pos, x_pos + spike_len, y_pos], fill=(st[0], st[1], st[2], 180), width=1)
        draw.line([x_pos, y_pos - spike_len, x_pos, y_pos + spike_len], fill=(st[0], st[1], st[2], 180), width=1)
        
    out_file = os.path.join(textures_dir, "T_Deep_Space_Stars.png")
    img.save(out_file, "PNG", optimize=True)
    print("Saved:", out_file)

# -------------------------------------------------------------
# 2. GENERATE EARTH NIGHT CITY LIGHTS (2048 x 1024)
# -------------------------------------------------------------
def generate_earth_night_lights():
    print("Generating Earth Night City Lights (2048x1024)...")
    width, height = 2048, 1024
    
    # Load Earth Albedo to locate land vs ocean
    albedo_path = os.path.join(textures_dir, "T_Planet_Earth_Albedo.png")
    earth_albedo = Image.open(albedo_path).convert("RGB").resize((width, height))
    albedo_data = np.array(earth_albedo)
    
    # Land mask: oceans are predominantly blue (blue > red + 15)
    r = albedo_data[:, :, 0].astype(float)
    g = albedo_data[:, :, 1].astype(float)
    b = albedo_data[:, :, 2].astype(float)
    is_ocean = (b > r + 18) & (b > g)
    is_land = ~is_ocean
    
    # City lights density pattern
    np.random.seed(101)
    noise1 = np.random.rand(height, width)
    noise2 = np.random.rand(height, width)
    
    # Latitude weighting: temperate zones have more cities (lat ~ 20 to 60 N/S)
    y_idx = np.arange(height)
    lat_deg = 90.0 - (y_idx / height) * 180.0
    lat_weight = np.exp(-((lat_deg - 38.0)**2) / 450.0) + 0.6 * np.exp(-((lat_deg - 15.0)**2) / 300.0) + 0.4 * np.exp(-((lat_deg + 25.0)**2) / 350.0)
    lat_grid = np.repeat(lat_weight[:, np.newaxis], width, axis=1)
    
    city_prob = is_land.astype(float) * lat_grid * (noise1 > 0.88).astype(float)
    
    # Golden amber city glow color: #FFE082 / #FFA726
    glow_r = np.clip(city_prob * 255 * (0.8 + 0.2 * noise2), 0, 255)
    glow_g = np.clip(city_prob * 190 * (0.8 + 0.2 * noise2), 0, 255)
    glow_b = np.clip(city_prob * 110 * (0.8 + 0.2 * noise2), 0, 255)
    
    # Metropolitan clusters (dilated glow)
    lights_img = Image.fromarray(np.stack([glow_r, glow_g, glow_b], axis=2).astype(np.uint8), "RGB")
    lights_blurred = lights_img.filter(ImageFilter.GaussianBlur(radius=1.5))
    
    # Combine sharp city points + diffuse suburban aura
    final_lights = np.array(lights_img).astype(float) * 1.5 + np.array(lights_blurred).astype(float) * 0.8
    final_lights = np.clip(final_lights, 0, 255).astype(np.uint8)
    
    # Alpha mask: black areas are transparent
    alpha = np.max(final_lights, axis=2)
    alpha = np.clip(alpha * 2.0, 0, 255).astype(np.uint8)
    
    rgba = np.dstack([final_lights, alpha])
    out_img = Image.fromarray(rgba, "RGBA")
    
    out_file = os.path.join(textures_dir, "T_Planet_Earth_NightLights.png")
    out_img.save(out_file, "PNG", optimize=True)
    print("Saved:", out_file)

# -------------------------------------------------------------
# 3. GENERATE REALISTIC MOON ALBEDO & NORMAL (2048 x 1024)
# -------------------------------------------------------------
def generate_moon_textures():
    print("Generating Moon Albedo and Normal Maps (2048x1024)...")
    width, height = 2048, 1024
    
    np.random.seed(77)
    x = np.linspace(-math.pi, math.pi, width)
    y = np.linspace(-math.pi/2, math.pi/2, height)
    xx, yy = np.meshgrid(x, y)
    
    # Lunar Maria (dark basaltic plains): Mare Tranquillitatis, Imbrium, Serenitatis, Oceanus Procellarum
    # Near side is x between -1.5 and 1.5
    maria1 = np.exp(-((xx + 0.6)**2 * 2.0 + (yy - 0.3)**2 * 6.0)) # Mare Imbrium
    maria2 = np.exp(-((xx - 0.4)**2 * 3.5 + (yy - 0.1)**2 * 8.0)) # Mare Tranquillitatis & Serenitatis
    maria3 = np.exp(-((xx + 1.2)**2 * 1.8 + (yy - 0.0)**2 * 5.0)) # Oceanus Procellarum
    maria4 = np.exp(-((xx - 1.1)**2 * 4.0 + (yy - 0.2)**2 * 9.0)) # Mare Crisium
    maria = np.clip((maria1 * 0.8 + maria2 * 0.75 + maria3 * 0.85 + maria4 * 0.6), 0.0, 1.0)
    
    # Lunar regolith fine noise & cratering
    n1 = (np.sin(xx * 15.0 + np.cos(yy * 12.0)) * 0.5 + 0.5)
    n2 = (np.sin(xx * 40.0 - yy * 35.0) * 0.5 + 0.5)
    n3 = np.random.rand(height, width)
    
    base_lunar = 145 + 35 * n1 + 15 * n2 + 10 * n3
    # Darken the maria
    base_lunar -= maria * 65.0
    
    albedo = np.clip(base_lunar, 40, 225).astype(np.uint8)
    
    # Render impact craters with bright ejecta rays (Tycho crater near south, Copernicus, Kepler)
    albedo_img = Image.fromarray(albedo, "L").convert("RGB")
    draw = ImageDraw.Draw(albedo_img)
    
    # Add crater list: (center_x, center_y, radius, ray_count)
    craters = [
        (int(width * 0.48), int(height * 0.75), 24, 16), # Tycho
        (int(width * 0.43), int(height * 0.38), 18, 12), # Copernicus
        (int(width * 0.35), int(height * 0.42), 14, 8),  # Kepler
        (int(width * 0.55), int(height * 0.32), 16, 6),  # Aristoteles
        (int(width * 0.78), int(height * 0.60), 22, 10), # Far side crater
        (int(width * 0.15), int(height * 0.55), 20, 8),  # Far side crater
        (int(width * 0.85), int(height * 0.35), 18, 8),
    ]
    
    # Add random smaller impact craters (200 craters)
    for _ in range(250):
        cx = np.random.randint(20, width - 20)
        cy = np.random.randint(20, height - 20)
        cr = np.random.randint(2, 8)
        craters.append((cx, cy, cr, 0))
        
    for cx, cy, cr, rays in craters:
        # Crater rim (bright)
        draw.ellipse([cx - cr, cy - cr, cx + cr, cy + cr], outline=(220, 220, 225), width=max(1, cr // 4))
        # Crater floor (dark)
        draw.ellipse([cx - cr + 1, cy - cr + 1, cx + cr - 1, cy + cr - 1], fill=(80, 80, 85))
        # Central peak
        if cr > 10:
            draw.ellipse([cx - 2, cy - 2, cx + 2, cy + 2], fill=(230, 230, 235))
        # Ejecta rays
        if rays > 0:
            for a in range(rays):
                angle = (a / rays) * 2 * math.pi + np.random.uniform(-0.1, 0.1)
                r_len = np.random.randint(cr * 4, cr * 10)
                end_x = int(cx + math.cos(angle) * r_len)
                end_y = int(cy + math.sin(angle) * r_len)
                draw.line([cx, cy, end_x, end_y], fill=(210, 210, 215), width=1)
                
    albedo_img = albedo_img.filter(ImageFilter.GaussianBlur(radius=0.7))
    out_albedo = os.path.join(textures_dir, "T_Moon_Albedo.png")
    albedo_img.save(out_albedo, "PNG", optimize=True)
    print("Saved:", out_albedo)
    
    # Generate Moon Normal Map from height
    gray_data = np.array(albedo_img.convert("L")).astype(float)
    # Sobel / finite differences for normal map
    dzdx = np.gradient(gray_data, axis=1) * 1.5
    dzdy = np.gradient(gray_data, axis=0) * 1.5
    
    nx = -dzdx
    ny = -dzdy
    nz = 1.0
    length = np.sqrt(nx**2 + ny**2 + nz**2)
    nx /= length
    ny /= length
    nz /= length
    
    norm_r = np.clip((nx * 0.5 + 0.5) * 255, 0, 255).astype(np.uint8)
    norm_g = np.clip((ny * 0.5 + 0.5) * 255, 0, 255).astype(np.uint8)
    norm_b = np.clip((nz * 0.5 + 0.5) * 255, 0, 255).astype(np.uint8)
    
    norm_img = Image.fromarray(np.stack([norm_r, norm_g, norm_b], axis=2), "RGB")
    out_normal = os.path.join(textures_dir, "T_Moon_Normal.png")
    norm_img.save(out_normal, "PNG", optimize=True)
    print("Saved:", out_normal)

# -------------------------------------------------------------
# 4. GENERATE HIGH-INTENSITY SOLAR CORONA (1024 x 1024)
# -------------------------------------------------------------
def generate_sun_corona():
    print("Generating Radiant Solar Corona (1024x1024)...")
    size = 1024
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    
    cx, cy = size // 2, size // 2
    x = np.arange(size) - cx
    y = np.arange(size) - cy
    xx, yy = np.meshgrid(x, y)
    dist = np.sqrt(xx**2 + yy**2)
    angle = np.arctan2(yy, xx)
    
    # Solar disk core (radius ~ 90)
    core_r = 85.0
    
    # Radiant coronal rays & flare spikes
    ray_freq1 = np.sin(angle * 12.0) * 0.5 + 0.5
    ray_freq2 = np.sin(angle * 28.0 + 1.2) * 0.5 + 0.5
    ray_freq3 = np.sin(angle * 4.0 - 0.5) * 0.5 + 0.5
    rays = ray_freq1 * 0.4 + ray_freq2 * 0.3 + ray_freq3 * 0.3
    
    # Radial falloff
    dist_norm = np.clip((dist - core_r) / (size * 0.45), 0.0, 1.0)
    glow = np.exp(-dist_norm * 3.5) * (0.7 + 0.5 * rays)
    
    # 4 major anamorphic lens flare beams along primary axes
    beam_x = np.exp(-(yy**2) / 60.0) * np.exp(-np.abs(xx) / 280.0)
    beam_y = np.exp(-(xx**2) / 60.0) * np.exp(-np.abs(yy) / 280.0)
    diag1 = np.exp(-((xx + yy)**2) / 100.0) * np.exp(-dist / 320.0)
    diag2 = np.exp(-((xx - yy)**2) / 100.0) * np.exp(-dist / 320.0)
    beams = (beam_x + beam_y + 0.6 * diag1 + 0.6 * diag2)
    
    total_intensity = np.clip(glow + beams * 0.7, 0.0, 1.0)
    
    # Core incandescent white -> golden yellow -> warm solar orange -> deep space fade
    cr = np.clip(total_intensity * 255 * 1.1, 0, 255).astype(np.uint8)
    cg = np.clip(total_intensity * 220 * 0.95, 0, 255).astype(np.uint8)
    cb = np.clip(total_intensity * 140 * 0.75, 0, 255).astype(np.uint8)
    alpha = np.clip(total_intensity * 255 * 1.2, 0, 255).astype(np.uint8)
    
    # Inside core is solid incandescent white
    inside_core = dist < core_r
    cr[inside_core] = 255
    cg[inside_core] = 255
    cb[inside_core] = 250
    alpha[inside_core] = 255
    
    corona_rgba = np.dstack([cr, cg, cb, alpha])
    out_img = Image.fromarray(corona_rgba, "RGBA")
    
    out_file = os.path.join(textures_dir, "T_Sun_Corona.png")
    out_img.save(out_file, "PNG", optimize=True)
    print("Saved:", out_file)

if __name__ == "__main__":
    generate_deep_space_stars()
    generate_earth_night_lights()
    generate_moon_textures()
    generate_sun_corona()
    print("All celestial textures generated successfully!")

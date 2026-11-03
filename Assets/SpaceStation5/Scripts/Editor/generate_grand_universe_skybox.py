import numpy as np
from PIL import Image, ImageDraw, ImageFilter
import os
import math

textures_dir = r"d:\MY proyects\Nasa spaces Apps\Assets\SpaceStation5\Textures"
os.makedirs(textures_dir, exist_ok=True)

def generate_masterpiece_universe():
    print("Generating Masterpiece Astronomical Universe Skybox (4096x2048)...")
    width, height = 4096, 2048
    np.random.seed(777)
    
    # Coordinates in spherical equirectangular projection
    # x in [-pi, pi], y in [-pi/2, pi/2]
    x_coords = np.linspace(-math.pi, math.pi, width, endpoint=False)
    y_coords = np.linspace(-math.pi/2, math.pi/2, height)
    xx, yy = np.meshgrid(x_coords, y_coords)
    
    # -------------------------------------------------------------
    # 1. THE MILKY WAY GALACTIC DISC & BULGE
    # -------------------------------------------------------------
    # Galactic plane tilted at ~34 degrees, continuous & seamless across 360 deg
    tilt = 0.58
    # Periodic in x: use sin(xx) and cos(xx) for 100% seamless wrapping
    gal_plane = yy - tilt * np.sin(xx + 0.4) * np.cos(yy * 0.35)
    gal_dist = np.abs(gal_plane)
    
    # Multi-component stellar density profile
    mw_thin_disc = np.exp(-gal_dist * 5.2) * 0.65
    mw_thick_disc = np.exp(-gal_dist * 2.4) * 0.35
    mw_halo = np.exp(-gal_dist * 1.1) * 0.15
    mw_disc = mw_thin_disc + mw_thick_disc + mw_halo
    
    # Galactic Center & Bulge (Sagittarius Region) - seamless periodic position
    core_dx = np.arctan2(np.sin(xx + 0.25), np.cos(xx + 0.25))
    core_dy = yy - 0.14
    core_dist = np.sqrt(core_dx**2 * 1.8 + core_dy**2 * 5.5)
    core_glow = np.exp(-core_dist * 2.0) * 1.85
    
    # Secondary spiral arm concentration (Carina-Centaurus / Cygnus)
    arm2_dx = np.arctan2(np.sin(xx - 2.1), np.cos(xx - 2.1))
    arm2_dy = yy + 0.28
    arm2_dist = np.sqrt(arm2_dx**2 * 2.2 + arm2_dy**2 * 4.5)
    arm2_glow = np.exp(-arm2_dist * 2.6) * 0.8
    
    # -------------------------------------------------------------
    # 2. REALISTIC INTERSTELLAR DUST LANES (Great Rift Absorption)
    # -------------------------------------------------------------
    # Low & high frequency organic turbulent veins periodic in x
    dust1 = np.sin(xx * 6.0 + np.sin(yy * 10.0)) * 0.5 + 0.5
    dust2 = np.sin(xx * 14.0 - yy * 8.0 + np.cos(xx * 4.0)) * 0.5 + 0.5
    dust3 = np.cos(xx * 9.0 + yy * 12.0) * 0.5 + 0.5
    dust_turb = dust1 * 0.5 + dust2 * 0.35 + dust3 * 0.15
    
    # Dark dust rifts absorb stellar light predominantly inside the central thin disc
    dust_intensity = dust_turb * (mw_thin_disc * 1.6 + core_glow * 0.7)
    dust_mask = np.clip(1.0 - dust_intensity * 1.9, 0.04, 1.0)
    
    # -------------------------------------------------------------
    # 3. INTERSTELLAR GAS & NEBULOSITY (H-Alpha & O-III)
    # -------------------------------------------------------------
    neb1 = (np.sin(xx * 4.0 + np.cos(yy * 5.0)) * 0.5 + 0.5) * (np.cos(yy * 3.0 + np.sin(xx * 3.0)) * 0.5 + 0.5)
    neb2 = (np.sin(xx * 9.0 - yy * 7.0) * 0.5 + 0.5) * (np.cos(xx * 5.0 + yy * 8.0))
    neb_gas = np.clip(neb1 * 0.7 + neb2 * 0.3, 0.0, 1.0) * mw_disc * dust_mask
    
    # Chromatic balance:
    # Deep space vacuum base: (RGB 2, 2, 4)
    # Milky Way disc: Soft pearlescent silver-white (RGB 36, 40, 54)
    # Core bulge: Rich luminous golden-ivory (RGB 95, 82, 58)
    # Nebular gas: Subtle deep crimson (H-alpha) & turquoise (O-III)
    r_chan = (mw_disc * 38.0 + core_glow * 105.0 + arm2_glow * 50.0 + neb_gas * 42.0) * dust_mask
    g_chan = (mw_disc * 42.0 + core_glow * 88.0 + arm2_glow * 46.0 + neb_gas * 22.0) * dust_mask
    b_chan = (mw_disc * 62.0 + core_glow * 65.0 + arm2_glow * 60.0 + neb_gas * 48.0) * dust_mask
    
    # Vacuum black pedestal
    r_chan = np.clip(r_chan + 2.0, 0, 255).astype(np.uint8)
    g_chan = np.clip(g_chan + 2.0, 0, 255).astype(np.uint8)
    b_chan = np.clip(b_chan + 3.8, 0, 255).astype(np.uint8)
    
    base_rgba = np.dstack([r_chan, g_chan, b_chan, np.full((height, width), 255, dtype=np.uint8)])
    sky_img = Image.fromarray(base_rgba, "RGBA")
    
    # -------------------------------------------------------------
    # 4. ASTRONOMICAL STARFIELD (45,000+ REALISTIC STARS)
    # -------------------------------------------------------------
    print("  Adding 45,000 realistic stars...")
    # Separate layer for stars to allow smooth antialiasing
    stars_layer = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    draw = ImageDraw.Draw(stars_layer)
    
    # Spectral classifications with accurate color temperatures:
    spectra = [
        (195, 220, 255),  # Hot Blue O/B (10000K-30000K, Rigel, Spica)
        (230, 242, 255),  # Brilliant White A (7500K-10000K, Sirius, Vega)
        (255, 255, 255),  # Pure Diamond White
        (255, 248, 225),  # Soft Cream F (6000K-7500K, Procyon, Canopus)
        (255, 235, 175),  # Warm Solar Yellow G (5200K-6000K, Sun, Capella)
        (255, 195, 125),  # Warm Amber K (3700K-5200K, Arcturus, Aldebaran)
        (255, 135, 95),   # Deep Crimson/Orange M (2400K-3700K, Betelgeuse, Antares)
    ]
    spectral_weights = [0.18, 0.28, 0.20, 0.14, 0.10, 0.07, 0.03]
    
    # (A) Faint Background Stars (38,000 microscopic pinpoints)
    # Higher density in galactic disc, thinning towards celestial poles
    num_faint = 38000
    fx = np.random.randint(0, width, num_faint)
    # Polar correction for equirectangular projection: sample latitude by inverse sine
    u_lat = np.random.uniform(-0.95, 0.95, num_faint)
    fy = np.clip(((np.arcsin(u_lat) / (math.pi/2) + 1.0) * 0.5 * height).astype(int), 0, height - 1)
    
    # Compute distance to galactic plane for each star
    y_norm = (fy / height) * math.pi - math.pi/2
    x_norm = (fx / width) * 2.0 * math.pi - math.pi
    d_mw = np.abs(y_norm - tilt * np.sin(x_norm + 0.4) * np.cos(y_norm * 0.35))
    
    # Probability acceptance: dense in Milky Way, authentic in deep space
    keep_prob = np.exp(-d_mw * 1.8) * 0.75 + 0.25
    accepted = np.random.rand(num_faint) < keep_prob
    
    faint_x = fx[accepted]
    faint_y = fy[accepted]
    faint_b = np.random.uniform(0.18, 0.85, len(faint_x))
    
    for px, py, b in zip(faint_x, faint_y, faint_b):
        c = int(b * 225)
        # Slight spectral tint
        tint_idx = np.random.choice(len(spectra), p=spectral_weights)
        s_col = spectra[tint_idx]
        col = (int(c * s_col[0] / 255.0), int(c * s_col[1] / 255.0), int(c * s_col[2] / 255.0), int(b * 255))
        draw.point((px, py), fill=col)
        
    # (B) Medium Apparent Magnitude Stars (6,500 prominent stars, radius 1.0-1.6)
    num_med = 6500
    mx = np.random.randint(0, width, num_med)
    u_lat_m = np.random.uniform(-0.92, 0.92, num_med)
    my = np.clip(((np.arcsin(u_lat_m) / (math.pi/2) + 1.0) * 0.5 * height).astype(int), 0, height - 1)
    
    for x_pos, y_pos in zip(mx, my):
        s_idx = np.random.choice(len(spectra), p=spectral_weights)
        col = spectra[s_idx]
        brightness = np.random.uniform(0.65, 1.0)
        core_col = (int(col[0] * brightness), int(col[1] * brightness), int(col[2] * brightness), 255)
        # 1-pixel diamond with 50% alpha halo
        draw.point((x_pos, y_pos), fill=core_col)
        if np.random.rand() > 0.4:
            draw.ellipse([x_pos - 1, y_pos - 1, x_pos + 1, y_pos + 1], fill=(col[0], col[1], col[2], 120))
            draw.point((x_pos, y_pos), fill=(255, 255, 255, 255))
            
    # (C) Open Star Clusters (Pleiades, Hyades, Perseus Double Cluster)
    clusters = [
        (int(width * 0.32), int(height * 0.36), 38, 55, (160, 205, 255)), # Pleiades
        (int(width * 0.68), int(height * 0.54), 30, 40, (190, 220, 255)), # Open cluster
        (int(width * 0.16), int(height * 0.60), 28, 35, (255, 220, 180)), # Hyades warm cluster
        (int(width * 0.88), int(height * 0.32), 24, 30, (200, 230, 255)), # Double cluster
    ]
    for cl_x, cl_y, cl_r, cl_count, cl_tint in clusters:
        # Faint diffuse reflection nebula
        neb_layer = Image.new("RGBA", (width, height), (0, 0, 0, 0))
        neb_draw = ImageDraw.Draw(neb_layer)
        for r_step in range(cl_r, 0, -4):
            alpha = int(22 * (1.0 - r_step / cl_r))
            neb_draw.ellipse([cl_x - r_step, cl_y - r_step, cl_x + r_step, cl_y + r_step],
                             fill=(cl_tint[0], cl_tint[1], cl_tint[2], alpha))
        neb_layer = neb_layer.filter(ImageFilter.GaussianBlur(cl_r * 0.3))
        stars_layer = Image.alpha_composite(stars_layer, neb_layer)
        draw = ImageDraw.Draw(stars_layer)
        
        # Cluster stars
        for _ in range(cl_count):
            ox = int(cl_x + np.random.normal(0, cl_r * 0.38))
            oy = int(cl_y + np.random.normal(0, cl_r * 0.38))
            if 0 <= ox < width and 0 <= oy < height:
                draw.ellipse([ox - 1, oy - 1, ox + 1, oy + 1], fill=(225, 240, 255, 240))
                draw.point((ox, oy), fill=(255, 255, 255, 255))

    # (D) Bright 1st & 2nd Magnitude Navigation Stars (80 prominent stars)
    # NO harsh lines! Only delicate, anti-aliased Gaussian glow halos
    print("  Rendering 80 bright navigation stars with soft optical halos...")
    num_bright = 80
    bx = np.random.randint(50, width - 50, num_bright)
    u_lat_b = np.random.uniform(-0.85, 0.85, num_bright)
    by = np.clip(((np.arcsin(u_lat_b) / (math.pi/2) + 1.0) * 0.5 * height).astype(int), 0, height - 1)
    
    halo_layer = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    halo_draw = ImageDraw.Draw(halo_layer)
    
    for i in range(num_bright):
        x_pos, y_pos = bx[i], by[i]
        s_idx = np.random.choice(len(spectra), p=spectral_weights)
        col = spectra[s_idx]
        radius = np.random.choice([2, 2, 3, 3, 4])
        
        # Soft multi-level luminous aura
        halo_r = radius * 4
        halo_draw.ellipse([x_pos - halo_r, y_pos - halo_r, x_pos + halo_r, y_pos + halo_r],
                          fill=(col[0], col[1], col[2], 40))
        halo_draw.ellipse([x_pos - radius * 2, y_pos - radius * 2, x_pos + radius * 2, y_pos + radius * 2],
                          fill=(col[0], col[1], col[2], 90))
        
        # Star core
        draw.ellipse([x_pos - radius, y_pos - radius, x_pos + radius, y_pos + radius],
                     fill=(col[0], col[1], col[2], 220))
        # Brilliant white center
        draw.ellipse([x_pos - max(1, radius - 1), y_pos - max(1, radius - 1), 
                      x_pos + max(1, radius - 1), y_pos + max(1, radius - 1)],
                     fill=(255, 255, 255, 255))
                     
    # Blur the soft auras to make them completely photorealistic
    halo_layer = halo_layer.filter(ImageFilter.GaussianBlur(3.0))
    
    # Composite all layers together
    final_img = Image.alpha_composite(sky_img, halo_layer)
    final_img = Image.alpha_composite(final_img, stars_layer)
    
    # Convert back to RGB for skybox
    final_rgb = final_img.convert("RGB")
    
    out_file = os.path.join(textures_dir, "T_Deep_Space_Stars.png")
    final_rgb.save(out_file, "PNG", optimize=True)
    print(f"Successfully generated and saved Grand Astronomical Universe to: {out_file} ({os.path.getsize(out_file)} bytes)")

if __name__ == "__main__":
    generate_masterpiece_universe()

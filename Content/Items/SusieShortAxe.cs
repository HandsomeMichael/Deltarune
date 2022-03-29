using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Deltarune.Content.Projectiles;
using Deltarune.Helper;

namespace Deltarune.Content.Items
{
	public class SusieShortAxe : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Short Menacing Axe"); 
			Tooltip.SetDefault("<right> to slash\nSlash attack inflict fatal bleed\nfatal bleed double true melee damage\nHitting enemy 10 time will summon flying axe\n'Crocodile pink tail axe'");
		}

		public override void SetDefaults() {
			item.damage = 20;
			item.axe = 10;
			item.melee = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 14;
			item.useAnimation = 14;
			item.useStyle = 1;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = 3;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.shootSpeed = 0f;
		}
		public override bool AltFunctionUse(Player player) {return true;}
		public override bool CanUseItem(Player player) {
			if (player.altFunctionUse == 2){
				//item.UseSound = SoundID.Item71;
				item.reuseDelay = 30;
				item.useStyle = 5;
				item.noMelee = true;
				item.noUseGraphic = true;
				item.shoot = ModContent.ProjectileType<SusieEpicSwing>();
				item.shootSpeed = 0f;
				Main.PlaySound(Deltarune.GetSound("swing"),player.Center);
				hit = 0;
			}
			else{
				item.reuseDelay = 0;
				//item.UseSound = SoundID.Item1;
				item.useStyle = 1;
				item.noMelee = false;
				item.noUseGraphic = false;
				item.shoot = 0;
				item.shootSpeed = 0f;
			}
			return base.CanUseItem(player);
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack){
			damage /= 2;
			knockBack = 3f;
			return true;
		}
		public int hit = 0;
		public override void ModifyHitNPC(Player player, NPC npc, ref int damage, ref float knockBack, ref bool crit) {
			hit++;
			if (hit % 10 == 0) {
				Main.PlaySound(Deltarune.GetSound("spellcast"),npc.Center);
				Projectile.NewProjectile(npc.Center,Vector2.Zero,ModContent.ProjectileType<SusieShortAxeThrown>(),damage/4,knockBack,player.whoAmI,npc.whoAmI);
			}
			damage += hit/3;
			if (hit >= 39) {
				CombatText.NewText(npc.getRect(),Color.Red,"Strike");
				Main.PlaySound(Deltarune.GetSound("damage"),npc.Center);
				player.CameraShake(10);
				damage *= 4;
				hit = 0;
			}
		}
		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,Color itemColor, Vector2 origin, float scale) {
			spriteBatch.BeginGlow(true,true);
			var texture = ModContent.GetTexture("Deltarune/Content/Items/SusieShortAxe_Glow");
			spriteBatch.Draw(texture, position, null, Color.White*((float)hit/40f), 0f, origin, scale, SpriteEffects.None, 0f);
			spriteBatch.BeginNormal(true,true);
		}
		/*
		public override bool CanRightClick() => true;
		public override void RightClick(Player player) {}
		public override bool ConsumeItem(Player player) => false;

		public override void UpdateInventory(Player player) {}
		public override bool UseItem(Player player){
			if (player.altFunctionUse == 2){}
			return true;
		}
		public override bool CanUseItem(Player player) {
			if (player.altFunctionUse == 2){}
			else{}
			return base.CanUseItem(player);
		}
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack){
			return true;
		}
		*/
	}
	public class SusieShortAxeThrown : ModProjectile
	{
		public override string Texture => "Deltarune/Content/Items/SusieShortAxe";
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Susi Epic");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;        //The recording mode
		}
		public override void SetDefaults() {
			//projectile.CloneDefaults(ProjectileID.WoodenBoomerang);
			projectile.width = 10;
			projectile.height = 10;
			projectile.melee = true;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.aiStyle = -1;
			projectile.alpha = 255;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			//aiType = ProjectileID.WoodenBoomerang;
		}
		Vector2 piss;
		public override void AI() {
			Player player = Main.player[projectile.owner];
			Dust d = Main.dust[Dust.NewDust(projectile.Center + new Vector2(Main.rand.NextFloat(-4,4),Main.rand.NextFloat(-4,4)), 0, 0, 88, 0f, 0f, 0, Color.White, 1f)];
			d.noGravity = true;
			if (projectile.ai[1] == -1f) {
				projectile.rotation = projectile.AngleTo(projectile.Center + projectile.velocity);
				projectile.rotation += MathHelper.ToRadians(30);
				projectile.alpha += 1;
				projectile.velocity = Vector2.Lerp(projectile.velocity,projectile.DirectionTo(piss)*10f,0.01f);
				if (projectile.alpha >= 255) {
					for (int i = 0; i < 20; i++){
						Vector2 v = projectile.velocity;
						Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 88, v.X, v.Y, 0, Color.White, 1f)];
						dust.noGravity = true;
					}
					projectile.Kill();	
				}
				return;	
			}

			NPC npc = Main.npc[(int)projectile.ai[0]];
			projectile.rotation = projectile.AngleTo(npc.Center);
			projectile.rotation += MathHelper.ToRadians(30);
			Vector2 pos = npc.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(projectile.ai[1]))*((float)Math.Sin(projectile.ai[1]/20f)*50f);
			projectile.Center = pos;

			if (projectile.alpha == 255) {
				for (int i = 0; i < Main.rand.Next(9,13); i++){
					Vector2 v = new Vector2(10,0);
					v.RotatedByRandom(MathHelper.ToRadians(360));
					Dust dust = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 88, v.X, v.Y, 0, Color.White, 1f)];
					dust.noGravity = true;
				}
			}
			//projectile.Center = Vector2.Lerp(projectile.Center,pos,0.1f);

			projectile.alpha -= 10;
			if (projectile.alpha < 0) {projectile.alpha = 0;}

			projectile.ai[1] += 5f;
			if (player.HeldItem.modItem != null && player.HeldItem.modItem is SusieShortAxe axe) {
				if (axe.hit == 0) {
					projectile.ai[1] = -1f;
					projectile.velocity = projectile.DirectionTo(npc.Center)*10f;
					piss = npc.Center;
					return;
				}
			}
			else {
				projectile.ai[1] = -1f;
				projectile.velocity = projectile.DirectionTo(npc.Center)*10f;
				piss = npc.Center;
				return;
			}
			if (!npc.active) {
				projectile.ai[1] = -1f;
				projectile.velocity = projectile.DirectionTo(player.Center)*10f;
				piss = npc.Center;
				return;
			}
			
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			//Redraw the projectile with the color not influenced by light
			var texture = Main.projectileTexture[projectile.type];
			for (int k = 0; k < projectile.oldPos.Length; k++) {
				Vector2 drawPos = projectile.Center(projectile.oldPos[k]) - Main.screenPosition;
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.oldRot[k], texture.Size()/2f, projectile.scale, SpriteEffects.None, 0f);
			}
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(Color.White), projectile.rotation, texture.Size()/2f, projectile.scale, SpriteEffects.None, 0f);
			spriteBatch.BeginGlow(true);
			texture = ModContent.GetTexture("Deltarune/Content/Items/SusieShortAxe_Glow");
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, texture.Size()/2f, projectile.scale, SpriteEffects.None, 0f);
			spriteBatch.BeginNormal(true);
			return false;
		}
	}
}
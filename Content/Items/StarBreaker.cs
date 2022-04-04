using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Deltarune.Helper;
using Deltarune.Content.Projectiles;
using Deltarune.Content.Dusts;
using Deltarune.Content.Buffs;

namespace Deltarune.Content.Items
{
	public class StarBreaker : ModItem
	{
		public override void SetStaticDefaults() {
			Tooltip.SetDefault("Hold to charge\nFully charged sword will inflict Lamebuf\nLamebuf reduce 80% enemy contact damage\n'Anti Social'");
		}
		public override void SetDefaults() {
			item.autoReuse = true;
			item.channel = true;
			item.melee = true;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.knockBack = 1f;
			item.damage = 30;
			item.width = 40;
			item.height = 40;
			item.useTime = 20;
			item.useAnimation = 20;
			item.shoot = ModContent.ProjectileType<StarBreakerProj>();
			item.shootSpeed = 3f;
			item.rare = 2;
			item.value = Item.buyPrice(gold: 1);
			item.useStyle = 1;
		}
	}
	public class StarBreakerProj : ModProjectile
	{
		public override string Texture => "Deltarune/Content/Items/StarBreaker";

		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Star Breaker");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;    //The length of old position to be recorded
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;        //The recording mode
		}

		public override void SetDefaults(){
			projectile.hostile = false;
			projectile.friendly = false;
			projectile.penetrate = -1;
			projectile.melee = true;
			projectile.width = 30;
			projectile.height = 30;
			projectile.aiStyle = -1;
			projectile.timeLeft = 60;
			projectile.tileCollide = false;
			projectile.extraUpdates = 1;
		}

		public float Charge {get => projectile.ai[0];set => projectile.ai[0] = value;}
		public float Phase {get => projectile.ai[1];set => projectile.ai[1] = value;}
		// mor faster than star absorber
		const float MaxCharge = 90f;

		public override void AI(){
			Player player = Main.player[projectile.owner];
			//bool shouldDed = Phase == 1f ? false : !player.channel;
			if (!player.active || player.dead || player.noItems || player.CCed || !player.channel || Charge > MaxCharge) {
				if (Phase == 0f) {
					// full charge will do something special
					Phase = 1f;
					if (Charge > MaxCharge) {
						Phase = 2f;
						player.CameraShake(4);
						Main.PlaySound(Deltarune.GetSound("spearrise"),projectile.Center);
						for (int a = 0; a < 20; a++) {
							Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
							Dust.NewDustPerfect(projectile.Center, ModContent.DustType<StarDust>(), speed * Main.rand.NextFloat(2f,5f), 1);
						}
					}
					else {
						Main.PlaySound(Deltarune.GetSound("smallswing"),projectile.Center);
					}
					float mult = ((Charge/MaxCharge)*1.5f);
					player.velocity += projectile.velocity*-0.2f;
					projectile.damage += (int)((float)projectile.damage*mult);
					projectile.friendly = true;
					// use the charge for penetrating
					Charge = (int)(Charge/(MaxCharge/5f)) + 1;
				}
			}
			UpdateGraphic(player);
		}

		void UpdateGraphic(Player player){
			if (Phase == 0f) {
				projectile.timeLeft = 600;
				Charge += 1f;
				player.heldProj = projectile.whoAmI;
				player.itemTime = 20;
				player.itemAnimation = 20;
				player.direction = projectile.direction;
				projectile.Center = player.Center - new Vector2(0,9);
				projectile.scale = 1f + ((Charge/MaxCharge)/2f);
				if (projectile.owner == Main.myPlayer) {
					float speed = ((Charge/MaxCharge)*15f) + 8f;
					projectile.velocity = projectile.DirectionTo(Main.MouseWorld)*speed;
					projectile.netUpdate = true;
				}
			}
			else {projectile.velocity.Y += 0.04f;}
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(projectile.direction == 1 ? 45 : -45);
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
			int penetrate = (int)Charge;
			if (Phase == 2f) {
				target.velocity *= 0.6f;
				// no maiden ????
				target.AddBuff<maidenless>(60*penetrate);
				for (int a = 0; a < 10; a++) {
					Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
					Dust.NewDustPerfect(target.Center, ModContent.DustType<StarDust>(), speed * Main.rand.NextFloat(3f,5f), 1);
				}
			}
			if (penetrate > 0) {
				int b = -1;
				Vector2 targetCenter = target.Center;
				for (int i = 0; i < Main.maxNPCs; i++) {
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy() && target.whoAmI != i) {
						float between = Vector2.Distance(npc.Center, projectile.Center);
						bool closest = Vector2.Distance(projectile.Center, targetCenter) > between;
						bool inRange = between < 700f;
						if ((closest && inRange) || b == -1) {
							b = i;
							targetCenter = npc.Center;
						}
					}
				}
				if (b != -1) {
					// slight homing and boosting, nobody gonna now lol. they goin to think that their aim is actually good
					projectile.velocity = projectile.velocity.Lerp(projectile.DirectionTo(targetCenter)*20f,0.1f);
				}
				penetrate--;
			}
			else {projectile.penetrate = 1;}
			Charge = penetrate;
		}
		public override bool ShouldUpdatePosition() => Phase != 0f;
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
			SpriteEffects spriteEffects = projectile.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
			Texture2D texture = Main.projectileTexture[projectile.type];
			for (int k = 0; k < projectile.oldPos.Length; k++) {
				Vector2 drawPos = projectile.Center(projectile.oldPos[k]) - Main.screenPosition;
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.oldRot[k], texture.Size()/2f, projectile.scale, spriteEffects, 0f);
			}
			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, texture.Size()/2f, projectile.scale, spriteEffects, 0);
			float HalfCharge = MaxCharge/2f;
			if (Phase == 0f && Charge > HalfCharge) {
				float alpha = (Charge-HalfCharge)/HalfCharge;
				texture = ModContent.GetTexture(Texture+"_overlay");
				spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.White*alpha, projectile.rotation, texture.Size()/2f, projectile.scale, spriteEffects, 0);
			}
			return false;
		}
	}
	// la la la la la
	// la ti las bara ti ti ti las
	// ti ti laa ta tilas ta ta ti ti ti las
	// la barias ata ti ti ti las 
	// ti ti lastis lastis la titilas
}

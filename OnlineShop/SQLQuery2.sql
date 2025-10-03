ALTER TABLE MatHang
ALTER COLUMN dongia NUMERIC(12, 0);

INSERT INTO MatHang (id_hang, id_loai, tenhang, donvitinh, dongia, soluongton, mota, hinhanh, tinhtrang) VALUES
('050', '5', 'Tai nghe Sony WH-1000XM5', 'Chiếc', 8500000, 30, 'Chống ồn chủ động, Bluetooth', '/images/sony_wh_1000xm5.jpg', 1),
('051', '5', 'Tai nghe Bose QuietComfort Ultra', 'Chiếc', 9000000, 28, 'Chống ồn hàng đầu, âm thanh không gian', '/images/bose_qc_ultra.jpg', 1),
('052', '5', 'Tai nghe Sennheiser Momentum 4 Wireless', 'Chiếc', 8000000, 32, 'Âm thanh audiophile, Bluetooth', '/images/sennheiser_momentum_4.jpg', 1),
('053', '5', 'Tai nghe Apple AirPods Pro (thế hệ 2)', 'Chiếc', 7500000, 35, 'Chống ồn chủ động, Spatial Audio', '/images/airpods_pro_2nd_gen.jpg', 1),
('054', '5', 'Tai nghe Beyerdynamic DT 770 PRO', 'Chiếc', 5000000, 40, 'Tai nghe kiểm âm, có dây', '/images/beyerdynamic_dt_770_pro.jpg', 1),
('055', '5', 'Tai nghe Sennheiser HD 800 S', 'Chiếc', 40000000, 3, 'Tai nghe over-ear cao cấp, âm thanh audiophile', '/images/sennheiser_hd_800_s.jpg', 1),
('056', '5', 'Tai nghe Focal Utopia', 'Chiếc', 120000000, 1, 'Tai nghe open-back tham chiếu', '/images/focal_utopia.jpg', 1),
('057', '5', 'Tai nghe Bowers & Wilkins PX8', 'Chiếc', 18000000, 15, 'Tai nghe không dây cao cấp, chống ồn', '/images/bowers_wilkins_px8.jpg', 1),
('058', '05', 'Tai nghe Shure AONIC 50 Gen 2', 'Chiếc', 15000000, 18, 'Tai nghe không dây chống ồn, chất âm chuyên nghiệp', '/images/shure_aonic_50_gen_2.jpg', 1);

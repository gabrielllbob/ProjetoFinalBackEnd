CREATE DATABASE IF NOT EXISTS `bancofraude`;
USE `railway`;

DROP TABLE IF EXISTS `transacoes`;
DROP TABLE IF EXISTS `mensalemprestimos`;
DROP TABLE IF EXISTS `emprestimos`;
DROP TABLE IF EXISTS `cartoes`;
DROP TABLE IF EXISTS `contas`;
DROP TABLE IF EXISTS `clientes`;

CREATE TABLE `clientes` (
  `ideCliente` bigint NOT NULL AUTO_INCREMENT,
  `nomCliente` varchar(50) NOT NULL,
  `nroCPF` char(11) NOT NULL,
  `nomEndereco` varchar(100) NOT NULL,
  `dtcNascimento` date NOT NULL,
  `stsAtivo` tinyint(1) NOT NULL DEFAULT '1',
  `senhaHash` varchar(255) NOT NULL,
  `role` enum('ADMIN','CLIENTE') DEFAULT 'CLIENTE',
  PRIMARY KEY (`ideCliente`),
  UNIQUE KEY `nroCPF` (`nroCPF`)
);

CREATE TABLE `contas` (
  `ideConta` bigint NOT NULL AUTO_INCREMENT,
  `nroSaldo` double NOT NULL DEFAULT 0,
  `selConta` set('Empresarial','Poupança','Corrente') DEFAULT NULL,
  `ideCliente` bigint DEFAULT NULL,
  `stsAtivo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`ideConta`),
  UNIQUE KEY `idx_cliente_tipo_unico` (`ideCliente`,`selConta`),
  CONSTRAINT `FK_Contas_clientes` FOREIGN KEY (`ideCliente`) REFERENCES `clientes` (`ideCliente`)
);

CREATE TABLE `cartoes` (
  `ideCartao` bigint NOT NULL AUTO_INCREMENT,
  `ideConta` bigint DEFAULT NULL,
  `nroCartao` char(12) NOT NULL DEFAULT '0',
  `selCartao` set('Debito','Credito') CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `nroVia` int NOT NULL DEFAULT '1',
  `nroLimite` int NOT NULL DEFAULT '0',
  `stsAtivo` tinyint(1) NOT NULL DEFAULT '1',
  `dtcCriacao` date NOT NULL DEFAULT (curdate()),
  `dtcVencimento` date NOT NULL DEFAULT ((curdate() + interval 5 year)),
  PRIMARY KEY (`ideCartao`),
  UNIQUE KEY `nroCartao` (`nroCartao`),
  UNIQUE KEY `idx_conta_tipo_cartao_unico` (`ideConta`,`selCartao`),
  CONSTRAINT `FK_cartoes_contas` FOREIGN KEY (`ideConta`) REFERENCES `contas` (`ideConta`)
);

CREATE TABLE `emprestimos` (
  `ideEmprestimo` bigint NOT NULL AUTO_INCREMENT,
  `ideConta` bigint DEFAULT NULL,
  `nroValor` int NOT NULL,
  `nroJuros` double NOT NULL DEFAULT 15,
  `nroTotalParcelas` double NOT NULL DEFAULT 36,
  `dtcEmprestimo` date NOT NULL DEFAULT (curdate()),
  `stsAtivo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`ideEmprestimo`),
  KEY `FK_emprestimos_contas` (`ideConta`),
  CONSTRAINT `FK_emprestimos_contas` FOREIGN KEY (`ideConta`) REFERENCES `contas` (`ideConta`)
);

CREATE TABLE `mensalemprestimos` (
  `ideMensalEmprestimo` bigint NOT NULL AUTO_INCREMENT,
  `ideEmprestimo` bigint DEFAULT NULL,
  `nroValor` int NOT NULL,
  `nroParcela` int NOT NULL DEFAULT '1',
  `dtcPagamento` date NOT NULL DEFAULT (curdate()),
  PRIMARY KEY (`ideMensalEmprestimo`),
  KEY `FK_mensalemprestimo_emprestimos` (`ideEmprestimo`),
  CONSTRAINT `FK_mensalemprestimo_emprestimos` FOREIGN KEY (`ideEmprestimo`) REFERENCES `emprestimos` (`ideEmprestimo`)
);

CREATE TABLE `transacoes` (
  `ideTransacao` bigint NOT NULL AUTO_INCREMENT,
  `selTransacao` set('Depósito','Saque','Transferência') DEFAULT NULL,
  `nroValor` double NOT NULL DEFAULT 0,
  `dtcTransacao` timestamp NULL DEFAULT (now()),
  `ideContaOrigem` bigint DEFAULT NULL,
  `ideContaFinal` bigint DEFAULT NULL,
  PRIMARY KEY (`ideTransacao`),
  KEY `FK_transacoes_clientes` (`ideContaOrigem`),
  KEY `FK_transacoes_clientes_2` (`ideContaFinal`),
  CONSTRAINT `FK_transacoes_contas` FOREIGN KEY (`ideContaOrigem`) REFERENCES `contas` (`ideConta`),
  CONSTRAINT `FK_transacoes_contas_2` FOREIGN KEY (`ideContaFinal`) REFERENCES `contas` (`ideConta`)
);

DROP PROCEDURE IF EXISTS spInsCliente;
CREATE PROCEDURE spInsCliente(
    IN pnomCliente VARCHAR(50),
    IN pnroCPF CHAR(11),
    IN pnomEndereco VARCHAR(100),
    IN pdtcNascimento DATE,
    IN psenhaHash VARCHAR(255),
    IN prole VARCHAR(20)
)
BEGIN
    INSERT INTO clientes 
    (nomCliente, nroCPF, nomEndereco, dtcNascimento, senhaHash, role)
    VALUES 
    (pnomCliente, pnroCPF, pnomEndereco, pdtcNascimento, psenhaHash, prole);
END;

DROP PROCEDURE IF EXISTS spInsConta;
CREATE PROCEDURE spInsConta(
    IN pnroSaldo DOUBLE,
    IN pselConta SET('Empresarial','Poupança','Corrente'),
    IN pideCliente BIGINT
)
BEGIN
    INSERT INTO contas (nroSaldo, selConta, ideCliente)
    VALUES (pnroSaldo, pselConta, pideCliente);
END;

DROP PROCEDURE IF EXISTS spInsCartao;
CREATE PROCEDURE spInsCartao(
    IN pideConta BIGINT
)
BEGIN
    DECLARE vprefixo CHAR(8);
    DECLARE vsufixo CHAR(4);
    DECLARE vnroCartao CHAR(12);
    DECLARE vmaxVia INT;

    SET vprefixo = LPAD(FLOOR(RAND()*99999999), 8, '0');
    SET vsufixo = '0001';
    SET vnroCartao = CONCAT(vprefixo, vsufixo);

    WHILE EXISTS (SELECT 1 FROM cartoes WHERE nroCartao = vnroCartao) DO
        SET vprefixo = LPAD(FLOOR(RAND()*99999999), 8, '0');
        SET vnroCartao = CONCAT(vprefixo, vsufixo);
    END WHILE;

    INSERT INTO cartoes (ideConta, nroCartao, nroVia, nroLimite, stsAtivo, dtcCriacao, dtcVencimento)
    VALUES (pideConta, vnroCartao, 1, 0, 1, CURDATE(), DATE_ADD(CURDATE(), INTERVAL 5 YEAR));
END;

DROP PROCEDURE IF EXISTS spInsEmprestimo;
CREATE PROCEDURE spInsEmprestimo(
    IN pideConta BIGINT,
    IN pnroValor DOUBLE,
    IN pnroJuros DOUBLE,
    IN pnroTotalParcelas DOUBLE
)
BEGIN
    INSERT INTO emprestimos (ideConta, nroValor, nroJuros, nroTotalParcelas)
    VALUES (pideConta, pnroValor, pnroJuros, pnroTotalParcelas);
END;

DROP PROCEDURE IF EXISTS spInsMensalEmprestimo;
CREATE PROCEDURE spInsMensalEmprestimo(
    IN pideEmprestimo BIGINT
)
BEGIN
    DECLARE vUltimaParcela INT;
    DECLARE vNovaParcela INT;
    DECLARE vTotalParcelas INT;
    DECLARE vAtivo BOOLEAN;
    DECLARE vValor DOUBLE;
    DECLARE vJuros DOUBLE;
    DECLARE vValorParcela DOUBLE;
    DECLARE vValorTotal DOUBLE;
    DECLARE vConta BIGINT;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
    END;

    START TRANSACTION;

    SELECT nroTotalParcelas, stsAtivo, nroValor, nroJuros, ideConta
    INTO vTotalParcelas, vAtivo, vValor, vJuros, vConta
    FROM emprestimos
    WHERE ideEmprestimo = pideEmprestimo
    FOR UPDATE;

    IF vTotalParcelas IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Empréstimo não encontrado';
    END IF;

    IF vAtivo = 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Empréstimo já encerrado';
    END IF;

    SELECT MAX(nroParcela)
    INTO vUltimaParcela
    FROM mensalemprestimos
    WHERE ideEmprestimo = pideEmprestimo;

    IF vUltimaParcela IS NULL THEN
        SET vNovaParcela = 1;
    ELSE
        SET vNovaParcela = vUltimaParcela + 1;
    END IF;

    IF vNovaParcela > vTotalParcelas THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Limite de parcelas já atingido';
    END IF;

    SET vValorTotal = vValor + (vValor * vJuros / 100);
    SET vValorParcela = vValorTotal / vTotalParcelas;

    INSERT INTO mensalemprestimos (ideEmprestimo, nroValor, nroParcela)
    VALUES (pideEmprestimo, vValorParcela, vNovaParcela);
    
    UPDATE contas c
    SET c.nroSaldo = c.nroSaldo - vValorParcela
    WHERE c.ideConta = vConta;

    IF vNovaParcela = vTotalParcelas THEN
        UPDATE emprestimos
        SET stsAtivo = 0
        WHERE ideEmprestimo = pideEmprestimo;
    END IF;

    COMMIT;
END;

DROP PROCEDURE IF EXISTS spInsTransacao;
CREATE PROCEDURE spInsTransacao(
    IN pselTransacao SET('Depósito','Saque','Transferência'),
    IN pnroValor DOUBLE,
    IN pideContaOrigem BIGINT,
    IN pideContaFinal BIGINT
)
BEGIN
    DECLARE vSaldoOrigem DOUBLE;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
    END;

    START TRANSACTION;

    IF pnroValor <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Valor deve ser maior que zero';
    END IF;

    IF pselTransacao = 'Depósito' THEN
        UPDATE contas
        SET nroSaldo = nroSaldo + pnroValor
        WHERE ideConta = pideContaOrigem;
    ELSEIF pselTransacao = 'Saque' THEN
        SELECT nroSaldo INTO vSaldoOrigem
        FROM contas
        WHERE ideConta = pideContaOrigem
        FOR UPDATE;

        IF vSaldoOrigem < pnroValor THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Saldo insuficiente';
        END IF;

        UPDATE contas
        SET nroSaldo = nroSaldo - pnroValor
        WHERE ideConta = pideContaOrigem;
    ELSEIF pselTransacao = 'Transferência' THEN
        SELECT nroSaldo INTO vSaldoOrigem
        FROM contas
        WHERE ideConta = pideContaOrigem
        FOR UPDATE;

        IF vSaldoOrigem < pnroValor THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Saldo insuficiente';
        END IF;

        UPDATE contas
        SET nroSaldo = nroSaldo - pnroValor
        WHERE ideConta = pideContaOrigem;

        UPDATE contas
        SET nroSaldo = nroSaldo + pnroValor
        WHERE ideConta = pideContaFinal;
    ELSE
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Tipo de transação inválido';
    END IF;

    INSERT INTO transacoes (selTransacao, nroValor, ideContaOrigem, ideContaFinal)
    VALUES (pselTransacao, pnroValor, pideContaOrigem, pideContaFinal);

    COMMIT;
END;

DROP PROCEDURE IF EXISTS spInsViaCartao;
CREATE PROCEDURE spInsViaCartao(
    IN pideCartao BIGINT
)
BEGIN
    DECLARE vprefixo CHAR(8);
    DECLARE vmaxVia INT;
    DECLARE vnroVia INT;
    DECLARE vnroCartao CHAR(12);

    SELECT LEFT(nroCartao, 8) INTO vprefixo
    FROM cartoes
    WHERE ideCartao = pideCartao;

    SELECT COALESCE(MAX(nroVia),0) INTO vmaxVia
    FROM cartoes
    WHERE LEFT(nroCartao,8) = vprefixo;

    SET vnroVia = vmaxVia + 1;
    SET vnroCartao = CONCAT(vprefixo, LPAD(vnroVia,4,'0'));

    INSERT INTO cartoes (ideConta, nroCartao, nroVia, nroLimite, stsAtivo, dtcCriacao, dtcVencimento)
    SELECT ideConta, vnroCartao, vnroVia, nroLimite, 1, CURDATE(), DATE_ADD(CURDATE(), INTERVAL 5 YEAR)
    FROM cartoes
    WHERE ideCartao = pideCartao;
END;

DROP PROCEDURE IF EXISTS spEdtCliente;
CREATE PROCEDURE spEdtCliente(
    IN pideCliente BIGINT,
    IN pnomCliente VARCHAR(50),
    IN pnroCPF CHAR(11),
    IN pnomEndereco VARCHAR(100),
    IN pdtcNascimento DATE
)
BEGIN
    UPDATE clientes c
    SET c.nomCliente = IF(pnomCliente = "", c.nomCliente, pnomCliente),
        c.nroCPF = IF(pnroCPF = "", c.nroCPF, pnroCPF),
        c.nomEndereco = IF(pnomEndereco = "", c.nomEndereco, pnomEndereco),
        c.dtcNascimento = IF(pdtcNascimento IS NULL, c.dtcNascimento, pdtcNascimento)
    WHERE c.ideCliente = pideCliente;
END;

DROP PROCEDURE IF EXISTS spEdtConta;
CREATE PROCEDURE spEdtConta(
    IN pnroSaldo DOUBLE,
    IN pselConta SET('Empresarial','Poupança','Corrente'),
    IN pideCliente BIGINT,
    IN pideConta BIGINT
)
BEGIN
    UPDATE contas c
    SET c.nroSaldo = IF(pnroSaldo = "", c.nroSaldo, pnroSaldo), 
        c.selConta = IF(pselConta = "", c.selConta, pselConta), 
        c.ideCliente = IF(pideCliente IS NULL, c.ideCliente, pideCliente)
    WHERE c.ideConta = pideConta;
END;

DROP PROCEDURE IF EXISTS spEdtCartao;
CREATE PROCEDURE spEdtCartao(
    IN pideConta BIGINT,
    IN pnroLimite INT
)
BEGIN
    UPDATE cartoes c
    SET c.ideConta = IF(pideConta IS NULL, c.ideConta, pideConta),
        c.nroLimite = IF(pnroLimite = "", c.nroLimite, pnroLimite)
    WHERE c.ideCliente = pideCliente;
END;

DROP PROCEDURE IF EXISTS spEdtStatusCliente;
CREATE PROCEDURE spEdtStatusCliente(
    IN pideCliente BIGINT,
    IN pstsAtivo BOOLEAN
)
BEGIN
    UPDATE clientes c
    SET c.stsAtivo = pstsAtivo
    WHERE c.ideCliente = pideCliente;
END;

DROP PROCEDURE IF EXISTS spEdtStatusConta;
CREATE PROCEDURE spEdtStatusConta(
    IN pstsAtivo BOOLEAN,
    IN pideConta BIGINT
)
BEGIN
    UPDATE contas c
    SET c.stsAtivo = pstsAtivo
    WHERE c.ideConta = pideConta;
END;

DROP PROCEDURE IF EXISTS spEdtStatusCartao;
CREATE PROCEDURE spEdtStatusCartao(
    IN pideCartao BIGINT,
    IN pstsAtivo BOOLEAN
)
BEGIN
    UPDATE cartoes c
    SET c.stsAtivo = pstsAtivo
    WHERE c.ideConta = pideConta;
END;

DROP PROCEDURE IF EXISTS spDelCliente;
CREATE PROCEDURE spDelCliente(
    IN pideCliente BIGINT
)
BEGIN
    UPDATE clientes c
    SET c.stsAtivo = 0
    WHERE c.ideCliente = pideCliente;
END;

DROP PROCEDURE IF EXISTS spDelConta;
CREATE PROCEDURE spDelConta(
    IN pideConta BIGINT
)
BEGIN
    DELETE FROM contas c
    WHERE c.ideConta = pideConta;
END;

DROP PROCEDURE IF EXISTS spDelCartao;
CREATE PROCEDURE spDelCartao(
    IN pideCartao BIGINT
)
BEGIN
    DELETE FROM cartoes c
    WHERE c.ideCartao = pideCartao;
END;

DROP PROCEDURE IF EXISTS spDelMensalEmprestimo;
CREATE PROCEDURE spDelMensalEmprestimo(
    IN pideMensalEmprestimo BIGINT
)
BEGIN
    DELETE FROM mensalemprestimos me
    WHERE me.ideMensalEmprestimo = pideMensalEmprestimo;
END;

DROP PROCEDURE IF EXISTS spSelClientes;
CREATE PROCEDURE spSelClientes()
BEGIN
    SELECT c.ideCliente, c.nomCliente, c.nroCPF, c.nomEndereco, c.dtcNascimento, c.stsAtivo
    FROM clientes c
    ORDER BY c.nomCliente;
END;

DROP PROCEDURE IF EXISTS spSelCliente;
CREATE PROCEDURE spSelCliente(IN pideCliente BIGINT)
BEGIN
    SELECT c.ideCliente, c.nomCliente, c.nroCPF, c.nomEndereco, c.dtcNascimento, c.stsAtivo
    FROM clientes c
    WHERE c.ideCliente = pideCliente
    ORDER BY c.nomCliente;
END;

DROP PROCEDURE IF EXISTS spSelContas;
CREATE PROCEDURE spSelContas()
BEGIN
    SELECT co.ideConta, FORMAT(co.nroSaldo, 2, 'pt_BR') AS nroSaldo, co.selConta, co.stsAtivo, cl.ideCliente, cl.nomCliente, cl.nroCPF
    FROM contas co JOIN clientes cl ON co.ideCliente = cl.ideCliente
    ORDER BY cl.nomCliente, co.nroSaldo;
END;

DROP PROCEDURE IF EXISTS spSelConta;
CREATE PROCEDURE spSelConta(
    IN pideConta BIGINT
)
BEGIN
    SELECT co.ideConta, FORMAT(co.nroSaldo, 2, 'pt_BR') AS nroSaldo, co.selConta, co.stsAtivo, cl.ideCliente, cl.nomCliente, cl.nroCPF
    FROM contas co JOIN clientes cl ON co.ideCliente = cl.ideCliente
    WHERE co.ideConta = pideConta
    ORDER BY cl.nomCliente, co.nroSaldo;
END;

DROP PROCEDURE IF EXISTS spSelCartoes;
CREATE PROCEDURE spSelCartoes()
BEGIN
    SELECT ca.ideCartao, ca.nroCartao, ca.nroVia, ca.nroLimite, ca.stsAtivo, ca.dtcCriacao, ca.dtcVencimento, co.ideConta, FORMAT(co.nroSaldo, 2, 'pt_BR') AS nroSaldo, co.selConta, cl.ideCliente, cl.nomCliente, cl.nroCPF
    FROM cartoes ca JOIN contas co ON ca.ideConta = co.ideConta
    JOIN clientes cl ON cl.ideCliente = co.ideCliente
    ORDER BY cl.nomCliente, co.nroSaldo;
END;

DROP PROCEDURE IF EXISTS spSelCartao;
CREATE PROCEDURE spSelCartao(
    IN pideCartao BIGINT
)
BEGIN
    SELECT ca.ideCartao, ca.nroCartao, ca.nroVia, ca.nroLimite, ca.stsAtivo, ca.dtcCriacao, ca.dtcVencimento, co.ideConta, FORMAT(co.nroSaldo, 2, 'pt_BR') AS nroSaldo, co.selConta, cl.ideCliente, cl.nomCliente, cl.nroCPF
    FROM cartoes ca JOIN contas co ON ca.ideConta = co.ideConta
    JOIN clientes cl ON cl.ideCliente = co.ideCliente
    WHERE ca.ideCartao = pideCartao	
    ORDER BY cl.nomCliente, co.nroSaldo;
END;

DROP PROCEDURE IF EXISTS spSelEmprestimos;
CREATE PROCEDURE spSelEmprestimos()
BEGIN
    SELECT e.ideEmprestimo, FORMAT(e.nroValor, 2, 'pt_BR') AS nroValor, e.nroJuros, e.nroTotalParcelas, e.dtcEmprestimo, e.stsAtivo, co.ideConta, FORMAT(co.nroSaldo, 2, 'pt_BR') AS nroSaldo, co.selConta, cl.ideCliente, cl.nomCliente, cl.nroCPF
    FROM emprestimos e JOIN contas co ON co.ideConta = e.ideConta
    JOIN clientes cl ON cl.ideCliente = co.ideCliente;
END;

DROP PROCEDURE IF EXISTS spSelEmprestimo;
CREATE PROCEDURE spSelEmprestimo(
    IN pideEmprestimo BIGINT
)
BEGIN
    SELECT e.ideEmprestimo, FORMAT(e.nroValor, 2, 'pt_BR') AS nroValor, e.nroJuros, e.nroTotalParcelas, e.dtcEmprestimo, e.stsAtivo, co.ideConta, FORMAT(co.nroSaldo, 2, 'pt_BR') AS nroSaldo, co.selConta, cl.ideCliente, cl.nomCliente, cl.nroCPF
    FROM emprestimos e JOIN contas co ON co.ideConta = e.ideConta
    JOIN clientes cl ON cl.ideCliente = co.ideCliente
    WHERE e.ideEmprestimo = pideEmprestimo;
END;

DROP PROCEDURE IF EXISTS spSelMensalEmprestimos;
CREATE PROCEDURE spSelMensalEmprestimos()
BEGIN
    SELECT me.ideMensalEmprestimo, me.nroValor, me.nroParcela, me.dtcPagamento, e.ideEmprestimo, FORMAT(e.nroValor, 2, 'pt_BR') AS nroValor, e.nroJuros, e.nroTotalParcelas, e.dtcEmprestimo, e.stsAtivo, co.ideConta, FORMAT(co.nroSaldo, 2, 'pt_BR') AS nroSaldo, co.selConta, cl.ideCliente, cl.nomCliente, cl.nroCPF
    FROM mensalemprestimos me JOIN emprestimos e ON me.ideEmprestimo = e.ideEmprestimo
    JOIN contas co ON co.ideConta = e.ideConta
    JOIN clientes cl ON cl.ideCliente = co.ideCliente;
END;

DROP PROCEDURE IF EXISTS spSelMensalEmprestimo;
CREATE PROCEDURE spSelMensalEmprestimo(
    IN pideEmprestimo BIGINT
)
BEGIN
    SELECT me.ideMensalEmprestimo, me.nroValor, me.nroParcela, me.dtcPagamento, e.ideEmprestimo, FORMAT(e.nroValor, 2, 'pt_BR') AS nroValor, e.nroJuros, e.nroTotalParcelas, e.dtcEmprestimo, e.stsAtivo, co.ideConta, FORMAT(co.nroSaldo, 2, 'pt_BR') AS nroSaldo, co.selConta, cl.ideCliente, cl.nomCliente, cl.nroCPF
    FROM mensalemprestimos me JOIN emprestimos e ON me.ideEmprestimo = e.ideEmprestimo
    JOIN contas co ON co.ideConta = e.ideConta
    JOIN clientes cl ON cl.ideCliente = co.ideCliente
    WHERE e.ideEmprestimo = pideEmprestimo;
END;

DROP PROCEDURE IF EXISTS spSelTransacoes;
CREATE PROCEDURE spSelTransacoes()
BEGIN
    SELECT t.ideTransacao, t.selTransacao, FORMAT(t.nroValor, 2, 'pt_BR') AS nroValor, t.dtcTransacao, t.ideContaOrigem, clo.nomCliente AS clienteOrigem, t.ideContaFinal, clf.nomCliente AS clienteDestino
    FROM transacoes t
    JOIN contas co_origem ON co_origem.ideConta = t.ideContaOrigem
    JOIN contas co_destino ON co_destino.ideConta = t.ideContaFinal
    JOIN clientes clo ON clo.ideCliente = co_origem.ideCliente
    JOIN clientes clf ON clf.ideCliente = co_destino.ideCliente;
END;

DROP PROCEDURE IF EXISTS spSelTransacoesCliente;
CREATE PROCEDURE spSelTransacoesCliente(
    IN pideConta BIGINT
)
BEGIN
    SELECT t.selTransacao, t.nroValor, t.dtcTransacao,
        CASE 
            WHEN t.selTransacao = 'Depósito' THEN 'Depósito em Dinheiro'
            ELSE COALESCE(clo.nomCliente, 'Sistema') 
        END AS clienteOrigem,
        CASE 
            WHEN t.selTransacao = 'Saque' THEN 'Retirada em Espécie'
            ELSE COALESCE(clf.nomCliente, 'Sistema') 
        END AS clienteDestino
    FROM transacoes t
    LEFT JOIN contas co_origem ON co_origem.ideConta = t.ideContaOrigem
    LEFT JOIN contas co_destino ON co_destino.ideConta = t.ideContaFinal
    LEFT JOIN clientes clo ON clo.ideCliente = co_origem.ideCliente
    LEFT JOIN clientes clf ON clf.ideCliente = co_destino.ideCliente
    WHERE t.ideContaOrigem = pideConta OR t.ideContaFinal = pideConta
    ORDER BY t.dtcTransacao DESC;
END;

DROP VIEW IF EXISTS vwcartoes;
CREATE VIEW vwcartoes AS
SELECT ca.ideCartao, ca.nroCartao, ca.nroVia, ca.nroLimite, ca.stsAtivo, ca.dtcCriacao, ca.dtcVencimento, co.ideConta, FORMAT(co.nroSaldo, 2, 'pt_BR') AS nroSaldo, co.selConta, cl.ideCliente, cl.nomCliente, cl.nroCPF
FROM cartoes ca JOIN contas co ON ca.ideConta = co.ideConta
JOIN clientes cl ON cl.ideCliente = co.ideCliente
WHERE ca.nroVia = (SELECT MAX(ca2.nroVia) FROM cartoes ca2 WHERE ca2.nroCartao = ca.nroCartao)
ORDER BY cl.nomCliente, co.nroSaldo;

INSERT INTO `clientes` (`nomCliente`, `nroCPF`, `nomEndereco`, `dtcNascimento`, `stsAtivo`, `senhaHash`, `role`)
SELECT 'Administrador Sistema', '00000000000', 'Endereço Padrão', '1990-01-01', 1, '$2a$12$HfusAIxDKqPl2W7Pn7c8q.29HX2F2MrDBhqA5FBnYrZP27mf5h49i', 'ADMIN'
WHERE NOT EXISTS (SELECT 1 FROM `clientes` WHERE `nroCPF` = '00000000000');